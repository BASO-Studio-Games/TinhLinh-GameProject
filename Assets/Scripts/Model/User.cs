
using System.Resources;

public class User {
    public string username;
    public string avatarUrl;
    public string diamond;

    public User(string username, string avatarUrl,  string diamond) {
        this.username = username;
        this.avatarUrl = avatarUrl;
        this.diamond = diamond;
    }
}
