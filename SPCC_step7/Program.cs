using System;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Security;

public enum PlayerState { Idle, kWalk, Run, Attack, Die }
public enum ItemType { Weapon, Armor, Potion, Etc }

public abstract class Character
{
    public string Name { get; private set; }
    public string Chad { get; private set; }
    public int Level { get; protected set; }
    public int Gold { get; set; }

    public int Hp { get; protected set; }
    public int MaxHp { get; protected set; }
    public int Str { get; protected set; }
    public int Def { get; protected set; }




    public Character(string name, string chad, int level, int str, int def, int hp, int maxhp, int gold)
    {
        Name = name;
        Chad = chad;
        Level = 1;
        Str = str;
        Def = def;
        Hp = hp;
        Gold = 1;
        MaxHp = maxhp;
    }

    public void Move()
    {
        Console.WriteLine("{Name}이(가) 이동합니다.");
    }

    public abstract void Die();
    //아빠 클래스의 추상클래스는 상속 받는 자식 클래스는 override나 자식 클래스에 구현, 혹은 
    //자식 클래스 자신도 추상클래스로 만들면 오류가 안 뜸

    public virtual void ShowInfo()
    {
        Console.WriteLine("########################################\r\n            S T A T U S\r\n########################################\r\n\n");
        Console.WriteLine($"   LV. {Level: D2} 이름: {Name} ({Chad})" +
            $"\n   EXP " +
            $"\n   HP     {Hp}" +
            $"\n   Str    {Str}" +
            $"\n   Def    {Def}" +
            $"\n   Gold   {Gold}G");
    }
}

public interface IAttackable
{
    void Attack(Character target);
}

class Player : Character, IAttackable
{
    public int Exp { get; set; }
    public int NextExp { get; private set; }

    public int Stamina { get; set; }
    public int MaxStamina { get; private set; }
    public int atk;
    public int LevelUpCount = 0;
    public PlayerState CurrentState { get; set; }

    public Player(string name, string chad, int level, int str, int def, int hp, int maxhp, int gold) : base(name, chad, level, str, def, hp, maxhp, gold)
    {
        this.Exp = 0;
        this.NextExp = this.Level * 10;
        this.Stamina = 20;
        this.MaxStamina = 20;
    }

    public void Attack()
    {
        Console.WriteLine($"{Name}이(가) 공격합니다!");
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Console.WriteLine($"{Name}이(가) {damage} 데미지를 입음. 남은 HP: {Hp}");
    }

    public void Attack(Character target)
    {
        Console.WriteLine($"{target.Name}에게 {atk}데미지!");
    }

    public override void Die()
    {
        Console.WriteLine("플레이어가 사망했습니다.");
    }

    public override void ShowInfo() //Character ShowInfo 출력을 override Player클래스 전용으로 재정의 함
    {
        Console.WriteLine("########################################\r\n            S T A T U S\r\n########################################\r\n\n");
        Console.Write($"   LV. {Level:D2} 이름: {Name} ({Chad})" +
            $"\n   EXP ");
        SPCC_STEP7.DrawProgressBar(Exp, NextExp);
        Console.WriteLine(" " + Exp + " / " + NextExp);

        Console.WriteLine("\n----------------------------------------\n");

        Console.Write(" HP  ");
        SPCC_STEP7.DrawProgressBar(Hp, MaxHp);
        Console.WriteLine(" " + Hp + " / " + MaxHp);
        Console.Write(" STA ");
        SPCC_STEP7.DrawProgressBar(Stamina, MaxStamina);
        Console.WriteLine(" " + Stamina + " / " + MaxStamina);

        Console.WriteLine("\n----------------------------------------\n");
        Console.WriteLine(
            $"\n   HP     {Hp}" +
            $"\n   Str    {Str}" +
            $"\n   Def    {Def}" +
            $"\n   Gold   {Gold}G");
    }
    public void LevelUp()
    {
        while (true)
        {
            Level++;

            this.Exp = Exp - NextExp;
            this.NextExp = this.Level * 10;
            this.Stamina = 20 + (this.Level * 5);
            this.MaxStamina = Stamina;
            LevelUpCount++;

            if (this.Exp < NextExp)
            {
                break;
            }

        }
        Console.WriteLine("★★★★★★  " + LevelUpCount + "레벨 업 ★★★★★★\n");
        LevelUpCount = 0;
    }
}
class Skill : IAttackable
{
    public string name;
    public int damage;
    public int mpCost;

    public Skill(string name, int damage, int mpCost)
    {
        this.name = name;
        this.damage = damage;
        this.mpCost = mpCost;
    }

    public void Activate()
    {
        Console.WriteLine($"스킬 {name} 발동! 데미지: {damage}, MP 소모: {mpCost}");
    }

    public void Attack(Character target)
    {
        Console.WriteLine($"스킬 {name}발동! {target.Name}에게 {damage}데미지!");
    }


}

class Monster : Character, IAttackable //몬스터 클래스
{
    public int level;

    public Monster(string name, string chad, int level, int str, int def, int hp, int maxhp, int gold) : base(name, chad, level, str, def, level * 50, maxhp, gold)
    {
        this.level = level;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"이름: {Name}, 레벨: {level}, HP: {Hp}");
    }

    public void Attack(Character target)
    {

    }

    public override void Die()
    {
        Console.WriteLine($"{Name} 몬스터가 사망했습니다.");
    }
}

class Item
{
    public string Name { get; private set; }
    public int str { get; private set; }
    public int def { get; private set; }
    public string info { get; private set; }
    public bool canEquip { get; set; }
    public int EquipType { get; set; }
    public int Gold { get; private set; }

    public Item(string Name, int str, int def, string info, int equipType, int gold)
    {
        this.Name = Name;
        this.str = str;
        this.def = def;
        this.info = info;
        canEquip = false;
        this.EquipType = equipType;
        this.Gold = Gold;
    }

    public void DisplayInfo()
    {
        int digits = 0;
        int width = 0;
        string displayInfo = ""; //가끔 30자가 넘는 설명을 가진 아이템을 위한 변수

        if (canEquip)
        {
            Console.Write("[E]");
        }
        else
        {
            Console.Write("   ");
        }
        Console.Write("| ");

        width = 10;
        digits = width - Name.Length;

        Console.Write($"{Name}");
        Console.Write(new string(' ', digits));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("|");
        Console.ResetColor();

        if (str > 0 && def == 0)
        {
            digits = width - str.ToString().Length;
            Console.Write("공격력");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" +");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{str} ");
            Console.ResetColor();
        }//공격력만 있는 장비
        else if (str == 0 && def > 0)
        {
            digits = width - def.ToString().Length;

            Console.Write("방어력");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" +");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{def} ");
            Console.ResetColor();
        }//방어력만 있는 장비
        /*else if (str > 0 && def > 0)
        {
            digits = width - (def.ToString().Length+ str.ToString().Length);
            Console.Write($"방어력 +{def} ");
            Console.Write(new string(' ', digits));
        } 2개 능력이 다 있는 아이템이 나올 때 까지 방치*/
        else
        {
            Console.Write($"{" ",-10}");
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("|");
        Console.ResetColor();


        Console.ForegroundColor = ConsoleColor.Gray;
        width = 30;
        digits = width - info.Length;

        if (digits < 0)
        {
            displayInfo = info.Substring(0, width - 3) + "...";
            Console.Write(displayInfo);
        }
        else
        {
            Console.Write(info);
            Console.Write(new string(' ', digits));
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("|");
        Console.ResetColor();
    }

}

class Shop
{
    public string Name { get; private set; }
    public int str { get; private set; }
    public int def { get; private set; }
    public string info { get; private set; }
    public bool canBuy { get; set; }
    public int EquipType { get; set; }
    public int Price { get; private set; }

    public Shop(string Name, int str, int def, string info, int equipType, int price)
    {
        this.Name = Name;
        this.str = str;
        this.def = def;
        this.info = info;
        canBuy = true;
        this.EquipType = equipType;
        this.Price = price;
    }

    public void DisplayInfo()
    {
        int digits = 0;
        int width = 0;
        string displayInfo = ""; //가끔 30자가 넘는 설명을 가진 아이템을 위한 변수      

        width = 10;
        digits = width - Name.Length;

        Console.Write($"{Name}");
        Console.Write(new string(' ', digits));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("|");
        Console.ResetColor();

        if (str > 0 && def == 0)
        {
            digits = width - str.ToString().Length;
            Console.Write("공격력");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" +");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{str} ");
            Console.ResetColor();
        }
        else if (str == 0 && def > 0)
        {
            digits = width - def.ToString().Length;

            Console.Write("방어력");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" +");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{def} ");
            Console.ResetColor();
        }
        /*else if (str > 0 && def > 0)
        {
            digits = width - (def.ToString().Length+ str.ToString().Length);
            Console.Write($"방어력 +{def} ");
            Console.Write(new string(' ', digits));
        } 2개 능력이 다 있는 아이템이 나올 때 까지 방치*/
        else
        {
            Console.Write($"{" ",-10}");
        }
        Console.Write("|");


        Console.ForegroundColor = ConsoleColor.Gray;
        width = 28;
        digits = width - info.Length;

        if (digits < 0)
        {
            displayInfo = info.Substring(0, width - 3) + "...";
            Console.Write(displayInfo);
        }
        else
        {
            Console.Write(info);
            Console.Write(new string(' ', digits));
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("|");
        Console.ResetColor();

        if (canBuy)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" {Price} G");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(" 구매완료");
        }
    }
}

class Explore
{
    //랜덤모험
    public void RndAdventure(Player player, int ExpTag)
    {
        if (ExpTag == 0)
        {
            Console.WriteLine("아무 일도 일어나지 않았다.\n");
            player.Stamina--;
        }
        else if (ExpTag == 1)
        {
            Console.WriteLine("몬스터 조우! 골드 500 획득\n");
            player.Gold += 500;
            player.Stamina--;

        }
    }
    //순찰
    public void Watch(Player player, int ExpTag)
    {
        if (ExpTag == 0)
        {
            Console.WriteLine("아무 일도 일어나지 않았다.\n");
            player.Stamina--;
        }
        else if (ExpTag == 1)
        {
            Console.WriteLine("마을 아이들이 모여있다. 간식을 사줘볼까?\n");
            player.Gold -= 500;
            player.Stamina--;

        }
        else if (ExpTag == 2)
        {
            Console.WriteLine("촌장님을 만나서 심부름을 했다.\n");
            player.Gold += 2000;
            player.Stamina--;

        }
        else if (ExpTag == 3)
        {
            Console.WriteLine("길 읽은 사람을 안내해주었다.\n");
            player.Gold += 1000;
            player.Stamina--;

        }
        else if (ExpTag == 4)
        {
            Console.WriteLine("마을 주민과 인사를 나눴다. 선물을 받았다.\n");
            player.Gold += 500;
            player.Stamina--;

        }
    }
    public void Trainig(Player player, int ExpTag)
    {
        if (ExpTag == 0)
        {
            Console.WriteLine("훈련이 잘 되었습니다.\n");
            player.Exp += 60;
            player.Stamina--;
        }
        else if (ExpTag == 1)
        {
            Console.WriteLine("오늘하루 열심히 훈련했습니다.\n");
            player.Exp += 40;
            player.Stamina--;

        }
        else if (ExpTag == 2)
        {
            Console.WriteLine("하기 싫다...훈련이...\n");
            player.Exp += 30;
            player.Stamina--;

        }
    }
}

class SPCC_STEP7
{

    public static void Main(string[] args)
    {
        //Monster slime = new Monster("슬라임", 1); //몬스터 인스턴스 생성
        Skill fireball = new Skill("Fireball", 50, 20); //파이어볼 스킬 인스턴스 생성
        Skill heal = new Skill("Heal", -30, 15); //힐 스킬 인스턴스 생성

        //Character player = new Player("DulLi", 74);
        //Character monster = new Monster("DDoChi", 12);

        string Choose;

        List<IAttackable> attackers = new List<IAttackable>();

        //attackers.Add(slime);
        attackers.Add(fireball);
        attackers.Add(heal);

        string name = "";
        string chad = "";
        int level = 999;
        int str = -1;
        int def = -1;
        int hp = -1;
        int gold = 99999999;

        bool exitAll = false; //다중반복문 탈출 용도 

        Random rand = new Random();
        int randomAction = 0;
        int ExpTag = -1;

        Console.WriteLine("게임 시작 전 이름과 직업을 선택해주세요");
        Console.Write("이름 : ");
        name = Console.ReadLine()!;
        //직업 선택 부분
        while (true)
        {
            Console.Write("1. 전사\n2. 궁수\n3. 탱커\n\n" +
                "원하 직업을 입력해주세요.\n >>  ");
            chad = Console.ReadLine()!;

            Console.Clear();
            if (chad == "1" || chad == "전사")
            {
                chad = "전사";
                str = 11;
                def = 9;
                hp = 10;
                break;
            }
            else if (chad == "2" || chad == "궁수")
            {
                chad = "궁수";
                str = 13;
                def = 3;
                hp = 6;
                break;
            }
            else if (chad == "3" || chad == "탱커")
            {
                chad = "탱커";
                str = 3;
                def = 15;
                hp = 12;
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("잘못된 입력입니다.\n");
            }
        }

        Character player2 = new Player(name, chad, level, str, def, hp, hp, gold);

        Item[] Inventory = new Item[10];
        Inventory[0] = new Item("무쇠갑옷", 0, 5, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1, 1500);
        Inventory[1] = new Item("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검 입니다.", 0, 510);
        Inventory[2] = new Item("연습용 창", 3, 0, "검보다는 그대로 창이 다루기 쉽죠.", 0, 150);
        Inventory[3] = new Item("빨간 거베라", 0, 0, "골목에 떨어져있던 의문의 꽃, " +
            "\"사랑에 빠지다\"라는 꽃말을 가지고 있다.", 5, 9999999);
        SPCC_STEP7.SortByNameAsc(Inventory);

        Shop[] shops = new Shop[10];
        shops[0] = new Shop("수련자 갑옷", 0, 5, "수련에 도움을 주는 갑옷입니다.", 1, 1000);
        shops[1] = new Shop("무쇠갑옷", 0, 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1, 1000);
        shops[2] = new Shop("스파르타의 갑옷", 0, 15, "스파르타의 전사들이 사용했다는 전슬의 갑옷입니다.", 1, 1000);
        shops[3] = new Shop("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검 입니다.", 0, 1000);
        shops[4] = new Shop("청동 도끼", 5, 0, "어디선가 사용됐던거 같은 도끼입니다.", 0, 1000);
        shops[5] = new Shop("스파르타의 창", 7, 0, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 0, 1000);

        /*
         0 - 무기
         1 - 방어구
         3 - ?(소비류 예정)
         4 - ?(중요 물건 예정)
         5 - 기타
         */

        Explore explore = new Explore();

    Start:
        Console.Clear();
        Console.WriteLine("========================================\n");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
        Console.ResetColor();
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다\n");
        Console.WriteLine("========================================\n\n");

        while (true)
        {
            Console.WriteLine("" +
                "1. 상태 보기 \n" +
                "2. 인벤토리 \n" +
                "3. 랜덤 모험\n" +
                "4. 마을 순찰하기\n" +
                "5. 훈련하기\n" +
                "6. 상점\n");
            Console.Write("현재 남은 스태미나\n");
            DrawProgressBar(((Player)player2).Stamina, ((Player)player2).MaxStamina);
            Console.WriteLine(" " + ((Player)player2).Stamina + " / " + ((Player)player2).MaxStamina + "\n");
            Console.Write("원하는 행동을 입력해주세요.\n >> ");


            Choose = Console.ReadLine()!;
            exitAll = false;
            Console.Clear();

            //상태창
            if (Choose == "1")
            {
                Console.Clear();
                player2.ShowInfo();
                Console.WriteLine("\n0. 나가기.\n");
                while (true)
                {
                    Console.Write("\n원하시는 행동을 입력해주세요\n>> ");
                    Choose = Console.ReadLine()!;
                    if (Choose == "0") //0 입력 시 상태창 종료
                    {
                        Console.Clear();
                        goto Start;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요");
                    }
                }

            }
            //인벤토리
            else if (Choose == "2")
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"인벤토리");
                    Console.ResetColor();

                    Console.WriteLine("########################################\r\n           I N V E N T O R Y\r\n########################################\r\n\n");
                    for (int i = 0; i < Inventory.Length; i++)
                    {
                        if (Inventory[i] != null)
                        {
                            Console.Write(" - ");
                            Inventory[i].DisplayInfo();
                        }
                    }

                    Console.WriteLine("\n 1. 장착 관리\n 2. 아이템 정렬\n 0. 나가기\n");
                    Console.Write(" 원하시는 행동을 입력해주세요\n >> ");
                    Choose = Console.ReadLine()!;
                    Console.Clear();

                    if (Choose == "0")
                    {
                        Console.Clear();
                        goto Start;
                    }
                    //아이템 장착
                    else if (Choose == "1")
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"인벤토리 - 아이템 장착");
                            Console.ResetColor();

                            Console.WriteLine("########################################\r\n           I N V E N T O R Y\r\n########################################\r\n\n");

                            for (int i = 0; i < Inventory.Length; i++)
                            {
                                if (Inventory[i] != null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(" - ");
                                    Console.ResetColor();

                                    Console.Write($"{i + 1} ");

                                    Inventory[i].DisplayInfo();

                                }
                            }

                            Console.WriteLine("\n0. 나가기\n");
                            Console.Write("원하시는 행동을 입력해주세요 \n >> ");

                            Choose = Console.ReadLine()!;

                            if (Choose == "0") // 나가기
                            {
                                Console.Clear();
                                break;
                            }

                            // 아이템앞에 있는 인덱스번호를 입력 시 장착메써드를 호출
                            else if (int.TryParse(Choose, out int num) && num <= Inventory.Length && num > 0)
                            {
                                Console.Clear();
                                SPCC_STEP7.EquipItem(Inventory, num - 1);
                            }

                            else
                            {
                                Console.Clear();
                                Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요\n");
                            }
                        }
                    }
                    //아이템 정렬
                    else if (Choose == "2")
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"인벤토리 - 아이템 정렬");
                            Console.ResetColor();

                            Console.WriteLine("########################################\r\n           I N V E N T O R Y\r\n########################################\r\n\n");

                            for (int i = 0; i < Inventory.Length; i++)
                            {
                                if (Inventory[i] != null)
                                {
                                    Console.Write($" - {i + 1} ");
                                    Inventory[i].DisplayInfo();
                                }
                            }

                            Console.WriteLine("\n" +
                                "1. 이름 - 짧은 순\n" +
                                "2. 이름 - 길은 순\n" +
                                "3. 장착순\n" +
                                "4. 공격력\n" +
                                "5. 방어력\n" +
                                "0. 나가기\n");
                            Console.Write("원하시는 행동을 입력해주세요 \n >> ");

                            Choose = Console.ReadLine()!;

                            if (Choose == "0") // 나가기
                            {
                                exitAll = true;
                                Console.Clear();
                            }

                            // 1. 짧은 순
                            else if (Choose == "1")
                            {
                                Console.Clear();
                                SortByNameAsc(Inventory);
                            }
                            // 2. 긴 순
                            else if (Choose == "2")
                            {
                                Console.Clear();
                                SortByNameDesc(Inventory);
                            }
                            // 장착순
                            else if (Choose == "3")
                            {
                                Console.Clear();
                                SortByEquip(Inventory);
                            }
                            //공격력
                            else if (Choose == "4")
                            {
                                Console.Clear();
                                SortByAttack(Inventory);
                            }
                            else if (Choose == "5")
                            {
                                Console.Clear();
                                SortByDefense(Inventory);
                            }
                            //방어력
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요\n");
                            }

                            if (exitAll)
                            {
                                Console.Clear();
                                break;
                            }
                        }
                        exitAll = false;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요");
                    }
                }

            }
            //랜덤모험
            else if (Choose == "3")
            {
                randomAction = rand.Next(1, 101);
                if (((Player)player2).Stamina > 0)
                {
                    if (randomAction < 50)
                    {
                        ExpTag = 0;
                        explore.RndAdventure((Player)player2, ExpTag);
                    }
                    else
                    {
                        ExpTag = 1;
                        explore.RndAdventure((Player)player2, ExpTag);
                    }
                }
                else
                {
                    Console.WriteLine("스태미나가 부족합니다\n");
                }
            }
            //마을순찰
            else if (Choose == "4")
            {
                randomAction = rand.Next(1, 101);

                if (((Player)player2).Stamina > 0)
                {
                    if (randomAction <= 30)
                    {
                        ExpTag = 0;
                        explore.Watch((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 40)
                    {
                        ExpTag = 1;
                        explore.Watch((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 50)
                    {
                        ExpTag = 2;
                        explore.Watch((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 70)
                    {
                        ExpTag = 3;
                        explore.Watch((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 100)
                    {
                        ExpTag = 4;
                        explore.Watch((Player)player2, ExpTag);
                    }
                }
                else
                {
                    Console.WriteLine("스태미나가 부족합니다");
                }
            }
            //훈련
            else if (Choose == "5")
            {
                randomAction = rand.Next(1, 101);

                if (((Player)player2).Stamina > 0)
                {
                    if (randomAction <= 15)
                    {
                        ExpTag = 0;
                        explore.Trainig((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 75)
                    {
                        ExpTag = 1;
                        explore.Trainig((Player)player2, ExpTag);
                    }
                    else if (randomAction <= 100)
                    {
                        ExpTag = 2;
                        explore.Trainig((Player)player2, ExpTag);
                    }
                }
                else
                {
                    Console.WriteLine("스태미나가 부족합니다");
                }

                if (((Player)player2).Exp >= ((Player)player2).NextExp)
                {
                    ((Player)player2).LevelUp();
                }
            }
            //상점
            else if (Choose == "6")
            {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"상점");
                Console.ResetColor();

                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n\n" +
                    "[보유 골드]");
                Console.WriteLine($"{player2.Gold} G\n");
                Console.WriteLine("[아이템 목록]");


                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" - ");
                        Console.ResetColor();

                        Console.Write($"{i + 1} ");

                        Inventory[i].DisplayInfo();

                    }
                }


                Console.WriteLine("\n 1. 아이템 구매\n 2. 아이템 판매\n 0. 나가기\n");
                Console.Write(" 원하시는 행동을 입력해주세요\n >> ");
                Choose = Console.ReadLine()!;
                Console.Clear();

                while (true)
                {
                    if (Choose == "0")
                    {
                        Console.Clear();
                        break;
                    }//나가기
                    else if (Choose == "1")
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"상점 - 아이템 구매");
                            Console.ResetColor();

                            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n\n" +
                                "[보유 골드]");
                            Console.WriteLine($"{player2.Gold} G\n");
                            Console.WriteLine("[아이템 목록]");

                            for (int i = 0; i < shops.Length; i++)
                            {
                                if (shops[i] != null)
                                {
                                    Console.Write(" - ");
                                    Console.Write(i + 1 + " ");
                                    shops[i].DisplayInfo();
                                }
                            }

                            Console.WriteLine("\n 0. 나가기\n");
                            Console.Write(" 원하시는 행동을 입력해주세요\n >> ");
                            Choose = Console.ReadLine()!;

                            if (int.TryParse(Choose, out int num) && num <= shops.Length && num > 0)
                            {
                                if (shops[num - 1].Price <= player2.Gold && shops[num - 1].canBuy == true) //gold가 충분하고 팔린 아이템이 아니면 구매 진행
                                {
                                    Console.Clear();
                                    player2.Gold -= shops[num - 1].Price;
                                    shops[num - 1].canBuy = false;
                                    Console.WriteLine("구매를 완료하였습니다.\n");
                                }
                                else if (shops[num - 1].Price > player2.Gold)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Gold가 부족합니다\n");
                                    Console.ResetColor();
                                }
                                else if (shops[num - 1].canBuy == false)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("이미 구매한 아이템입니다.\n");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("잘못된 입력입니다.\n");
                                }
                            }
                            else if (Choose == "0")
                            {
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("잘못된 입력입니다.\n");
                            }
                        }
                    }//아이템 구매
                    else if (Choose == "2")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"상점 - 아이템 판매");
                        Console.ResetColor();

                        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n\n" +
                            "[보유 골드]");
                        Console.WriteLine($"{player2.Gold} G\n");
                        Console.WriteLine("[아이템 목록]");

                        for (int i = 0; i < shops.Length; i++)
                        {
                            if (shops[i] != null)
                            {
                                Console.Write(" - ");
                                Console.Write(i + 1 + " ");
                                shops[i].DisplayInfo();
                            }
                        }
                    }//아이템 판매
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("잘못된 입력입니다.\n");
                    }
                }
            }
            //오입력
            else
            {
                Console.Clear();
                Console.WriteLine("잘못된 입력입니다.\n");
            }
        }


    }

    public static void DrawProgressBar(int Current, int max)
    {
        int width = 20;
        Console.Write("[");

        int ExpBar = (int)Math.Round((double)Current / max * width);

        Console.Write(new string('#', ExpBar));
        Console.Write(new string('-', width - ExpBar));
        Console.Write("]");
    }

    public static void EquipItem(Item[] item, int target) // 같은 종류의 장비의 중복 장착을 막을 메써드
    {
        int temp = -1;
        if (item[target].EquipType < 5)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] != null && i != target && item[target].EquipType == item[i].EquipType && item[i].canEquip == true)
                {
                    temp = i;
                }

            }

            if (temp != -1)
            {
                item[temp].canEquip = !item[temp].canEquip;
            }

            item[target].canEquip = !item[target].canEquip;
        }

        else if (item[target].EquipType == 5)
        {
            Console.WriteLine("장착 할 수 없는 아이템 입니다.");
        }
    }

    public static void SortByNameAsc(Item[] item)
    {
        Array.Sort(item, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.Name.Length.CompareTo(b.Name.Length);
        });
    }// 이름 짧은 순

    public static void SortByNameDesc(Item[] item)
    {
        Array.Sort(item, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            return b.Name.Length.CompareTo(a.Name.Length);
        });
    }// 이름 긴 순

    public static void SortByEquip(Item[] item)
    {
        int index = 0;
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] != null && item[i].canEquip == true)
            {

                (item[index], item[i]) = (item[i], item[index]);
                index++;

            }
        }
    }//장착된 아이템 순서

    public static void SortByAttack(Item[] item)
    {
        Array.Sort(item, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            return b.str.CompareTo(a.str);
        });
    }//공격력이 높은 순

    public static void SortByDefense(Item[] item)
    {
        Array.Sort(item, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            return b.def.CompareTo(a.def);
        });


    }//방어력이 높은 순
}