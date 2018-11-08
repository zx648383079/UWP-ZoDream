using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public abstract class ChatItem
    {
        public DateTime Published { get; set; }
    }

    public class TimeAxis : ChatItem, INotifyPropertyChanged
    {
        private string _content;

        public string Content
        {
            get { return _content; }
            set {
                _content = value;
                SetPropertyChanged("Content");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void SetPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

    }

    public class MessageBase:ChatItem
    {
        public string Name { get; set; }

        public string Avatar { get; set; }

        public bool IsSelf { get; set; } = false;
    }

    public class Message : MessageBase
    {
        public string Content { get; set; }
    }

    public class GiftMessage : MessageBase
    {
        public double Amount { get; set; }
    }

    public class ImageMessage : MessageBase
    {
        public string Source { get; set; }
    }

    public class VoiceMessage : ImageMessage
    {

    }

    public class VideoMessage : ImageMessage
    {

    }

    public class FileMessage : ImageMessage
    {

    }
}
