using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using CloudStructures;
using StackExchange.Redis;
using ZLogger;


class Redis
{
    private ConnectionMultiplexer redisConeection;
    private IDatabase db;

    public Redis()
    {
    }

    RedisConnection rc;
    ~Redis()
    {

    }
    public bool Initialize(string host, int port)
    {
        try
        {
            this.redisConeection = ConnectionMultiplexer.Connect(host + ":" + port);
            if (this.redisConeection.IsConnected)
            {
                this.db = this.redisConeection.GetDatabase();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public void GetString(string key)
    {
        Console.WriteLine(db.StringGet(key));
    }
    public void SetString(string key, string val)
    {
        if (this.db.StringSet(key, val)) Console.WriteLine(db.StringGet(key + ":" + val));

    }
}

class Program
{
    static void Main(string[] args)
    {
        Redis redis = new Redis();
        if (redis.Initialize("127.0.0.1", 6379))
        {
            Console.WriteLine("Redis Connected");
            while (true)
            {
                Console.Write("KEY: ");
                string key = Console.ReadLine();
                Console.WriteLine("");
                Console.Write("Value:");
                string value = Console.ReadLine();
                redis.SetString(key, value);
                Console.ReadKey();
            }
        }
        else return;

    }
}


