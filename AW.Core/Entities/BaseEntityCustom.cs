using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Core.Entities.Interface;

namespace AW.Core.Entities
{
    public class BaseEntityCustom : IBaseEntityStandard
    {
        public string LogBy { get; set; } = string.Empty;
        public string LogByUserDisplayName { get; set; } = string.Empty;
        public DateTime LogDate { get; set; } = DateTime.Now;
        private DateTime _LogDateUTC;
        public DateTime LogDateUTC
        {
            get
            {
                return _LogDateUTC;
            }
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _LogDateUTC = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
                else
                {
                    _LogDateUTC = value;
                }
            }
        }
        public int LogInc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserDisplayName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        private DateTime _CreatedDateUTC;
        public DateTime CreatedDateUTC
        {
            get
            {
                return _CreatedDateUTC;
            }
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _CreatedDateUTC = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
                else
                {
                    _CreatedDateUTC = value;
                }
            }
        }
        public string Remarks { get; set; } = string.Empty;
        public bool Disabled { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
