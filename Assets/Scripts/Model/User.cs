
using System.Resources;

public class User {
    public string username;
    public string avatarUrl;
    public string diamond;
    public string roll;
    public string currentLevel;

    public User(string username, string avatarUrl,  string diamond, string roll, string currentLevel) {
        this.username = username;
        this.avatarUrl = avatarUrl;
        this.diamond = diamond;
        this.roll = roll;
        this.currentLevel = currentLevel;
    }
}
