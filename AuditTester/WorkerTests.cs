namespace AuditTester;

[TestClass]
public class WorkerTests
{
    private const int MinLengthName = 6;
    private const int MaxLengthName = 64;
    
    private const int DateMaxYear = 2050;
    private const int DateMinYear = 1950;
    
    [TestMethod]
    public void EmptyName_CheckName()
    {
        var actual = "";
        try {
            Worker.CheckName("");
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual($"ФИО должно содержать минимум {MinLengthName} символов", actual, "Неправильная проверка пустого имени");
    }
    
    [TestMethod]
    public void LongName_CheckName()
    {
        var actual = "";
        const string name = "aaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        try {
            Worker.CheckName(name);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual($"ФИО должно содержать максимум {MaxLengthName} символов", actual, "Неправильная проверка длинного имени");
    }
    
    [TestMethod]
    public void InvalidCharacters_CheckName()
    {
        var actual = "";
        const string invalidCharacters = "][/,.'!@#$%^&*()_+=-123467890";
        try {
            foreach (var symbol in invalidCharacters)
                Worker.CheckName(symbol + "aaaaaa");
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.IsTrue(actual.Contains("ФИО содержит недопустимые символы -"), "Неправильная проверка недопустимых символов в имени");
    }
    
    [TestMethod]
    public void IncorrectFormat_CheckName()
    {
        var actual = "";
        const string name = "Vladimir PupkinVasilevich";
        try {
            Worker.CheckName(name);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("ФИО не соответствует формату", actual, "Неправильная проверка формата имени");
    }
    
    [TestMethod]
    public void ValidName_CheckName()
    {
        var actual = "";
        const string name = "Vladimir Pupkin Vasilevich";
        try {
            Worker.CheckName(name);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("", actual, "Неправильная проверка имени");
    }
    
    [TestMethod]
    public void EmptyPassport_CheckPassport()
    {
        var actual = "";
        const string passport = "";
        try {
            Worker.CheckPassport(passport);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Паспорт не соответствует формату", actual, "Неправильная проверка формата пасспорта");
    }
    
    [TestMethod]
    public void IncorrectPassport_CheckPassport()
    {
        var actual = "";
        var passports = new List<string>
        {
            "WASD 543657",
            "0534545454",
            "passpart",
            "432243 4324",
            "    4343 345543"
        };
        try {
            foreach (var passport in passports)
                Worker.CheckPassport(passport);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Паспорт не соответствует формату", actual, "Неправильная проверка формата пасспорта");
    }
    
    [TestMethod]
    public void ValidPassport_CheckPassport()
    {
        var actual = "";
        const string passport = "5436 543657";
        try {
            Worker.CheckPassport(passport);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("", actual, "Неправильная проверка формата пасспорта");
    }
    
    [TestMethod]
    public void EmptyBirthday_CheckBirthday()
    {
        var actual = "";
        const string birthday = "";
        try {
            Worker.CheckBirthday(birthday);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Дата рождения не соответствует формату", actual, "Неправильная проверка формата пасспорта");
    }
    
    [TestMethod]
    public void IncorrectBirthday_CheckBirthday()
    {
        var actual = ""; 
        var birthdays = new List<string>()
        {
            "244.52.2344",
            "31.13.2000",
            "32.01.2000",
            "11,12,2000",
            "--.--.----",
            "textUndef",
            "1.1.1",
            "..."
        };
        try {
            foreach (var birthday in birthdays)
                Worker.CheckBirthday(birthday);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Дата рождения не соответствует формату", actual, "Неправильная проверка формата даты рождения");
    }
    
    [TestMethod]
    public void SmallYearDate_CheckBirthday()
    {
        var actual = "";
        const string birthday = "01.01.1901";
        try {
            Worker.CheckBirthday(birthday);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual($"Год не может быть меньше {DateMinYear}", actual, "Неправильная проверка минимального года рождения");
    }
    
    [TestMethod]
    public void BigYearDate_CheckBirthday()
    {
        var actual = "";
        const string birthday = "01.01.2301";
        try {
            Worker.CheckBirthday(birthday);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual($"Год не может быть больше {DateMaxYear}", actual, "Неправильная проверка минимального года рождения");
    }
    
    [TestMethod]
    public void ValidDate_CheckBirthday()
    {
        var actual = "";
        const string birthday = "14.07.2003";
        try {
            Worker.CheckBirthday(birthday);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("", actual, "Неправильная проверка даты рождения");
    }
    
    [TestMethod]
    public void EmptyPhone_CheckPhoneNumber()
    {
        var actual = "";
        const string phone = "";
        try {
            Worker.CheckPhoneNumber(phone);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Номер телефона не соответствует формату", actual, "Неправильная проверка номера телефона");
    }
    
    [TestMethod]
    public void IncorrectPhone_CheckPhoneNumber()
    {
        var actual = "";
        var phones = new List<string>
        {
            "3490588935465647",
            "5445",
            "gfd45547334",
            "+5342564",
            "+79045958307", //Plus start
            "textUndef",
            "1",
            "09281948347" //Zero start
        };
        try {
            foreach (var phone in phones)
                Worker.CheckPhoneNumber(phone);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("Номер телефона не соответствует формату", actual, "Неправильная проверка номера телефона");
    }
    
    [TestMethod]
    public void ValidPhone_CheckPhoneNumber()
    {
        var actual = "";
        const string phone = "79081111111";
        try {
            Worker.CheckPhoneNumber(phone);
        } catch (Exception e) {
            actual = e.Message;
        }
        
        Assert.AreEqual("", actual, "Неправильная проверка номера телефона");
    }
}