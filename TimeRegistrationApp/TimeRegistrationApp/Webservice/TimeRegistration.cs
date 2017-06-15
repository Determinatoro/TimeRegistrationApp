using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeRegistrationApp.Webservice
{
    public class TimeRegistration
    {
        private int timeRegId;
        private string startTime;
        private string endTime;
        private string total;
        private int orderId;
        private int userId;
        private string description;
        private string note;

        private Brush background;

        public TimeRegistration()
        {

        }

        public int TimeRegId
        {
            get
            {
                return timeRegId;
            }

            set
            {
                timeRegId = value;
            }
        }

        public string StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
            }
        }

        public string EndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                endTime = value;
            }
        }

        public string Total
        {
            get
            {
                return total;
            }

            set
            {
                total = value;
            }
        }

        public int OrderId
        {
            get
            {
                return orderId;
            }

            set
            {
                orderId = value;
            }
        }

        public int UserId
        {
            get
            {
                return userId;
            }

            set
            {
                userId = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public string Note
        {
            get
            {
                return note;
            }

            set
            {
                note = value;
            }
        }

        public Brush Background
        {
            get
            {
                if (startTime == "")
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

                DateTime dateTime = DateTime.Parse(startTime);

                switch (dateTime.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x7F, 0x50));
                    case DayOfWeek.Monday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0x98, 0xFF, 0x98));                        
                    case DayOfWeek.Tuesday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x99, 0xFF));
                    case DayOfWeek.Wednesday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xF3, 0x80));
                    case DayOfWeek.Thursday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0x89, 0xC3, 0x5C));
                    case DayOfWeek.Friday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0xFE, 0xFF));
                    case DayOfWeek.Saturday:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0x5D, 0x59));
                    default:
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                }                
            }
        }
    }
}
