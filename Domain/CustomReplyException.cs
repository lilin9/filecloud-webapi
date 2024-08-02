using System.ComponentModel;

namespace Domain {
    public class CustomReplyException(string msg) : WarningException(msg);
}
