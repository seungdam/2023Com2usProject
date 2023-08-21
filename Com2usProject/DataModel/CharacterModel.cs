namespace Com2usProject.DataModel;

public class CharacterModel
{

    public string Nickname { get; set; }

    // 게임에서 사용하는 임의의 데이터들 
    public int Level { get; set; }

    public int Hp { get; set; }
    public int Mp { get; set; }

    public int CurExp { get; set; }
    public int MaxExp { get; set; }

    public int[] Inventory { get; set; }
}
