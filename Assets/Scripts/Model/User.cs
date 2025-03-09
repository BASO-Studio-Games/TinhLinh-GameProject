
using System.Resources;

public class User {
    public string username;
    public string avatarUrl;
    public string diamond;
    public string roll;

    public User(string username, string avatarUrl,  string diamond, string roll) {
        this.username = username;
        this.avatarUrl = avatarUrl;
        this.diamond = diamond;
        this.roll = roll;
    }
}
