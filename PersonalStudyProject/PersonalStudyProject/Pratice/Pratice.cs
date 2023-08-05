using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using CloudStructures;
using StackExchange.Redis;
using ZLogger;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;

public sealed class RedisServer // 싱글톤으로 구현
{

    public static RedisConnection m_redisConnection { get; set; }
    public static RedisConfig m_redisConfig { get; set; }
    private static readonly Lazy<RedisServer> _instance = new Lazy<RedisServer>(() => new RedisServer()); // thread safe 한 싱글톤 패턴을 구현하기 위해서 Lazy 키워드 사용
    private RedisServer() { }

    
    ~RedisServer()
    {

    }
  
    public static RedisServer GetInstance
    {
        get
        {
            return _instance.Value;
        }
    }

    public static void  Initialize(string conFigName,string ipAddress)
    {
        m_redisConfig = new RedisConfig(conFigName, ipAddress);
        m_redisConnection = new RedisConnection(m_redisConfig);
    }

}

class User
{
    public string m_id { get; set; }
    public string m_password { get; set; }
    public string m_nickName { get; set; }


    public User(string id, string passwd, string NickName) { m_id = id; m_password = passwd; m_nickName = NickName;  }
    public User() { }
    ~User() { }

}


class Program
{
    string idx = "0";

    public static ILogger GLogger;

    static async Task<int> Main(string[] args)
    {
        var rs = RedisServer.GetInstance;
        RedisServer.Initialize("Test","127.0.0.1");


        while (true)
        {
            
            Console.Write("ID : ");
            string id = Console.ReadLine();
            Console.Write("PW : ");
            string pw = Console.ReadLine();
            Console.Write("NickName : ");
            string nn = Console.ReadLine();
            var newUser = new User(id,pw,nn);
            var newData = new RedisString<User>(RedisServer.m_redisConnection, id, null);
            var result = await newData.SetAsync(newUser, null, When.NotExists); // 존재하지 않을 때만 수행하게 하기
       
            if (result)
            {
                Console.WriteLine("Register Success");
                Console.Clear();
            }
        }
    }
}


