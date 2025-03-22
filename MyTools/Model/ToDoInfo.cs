using LiteDB;
using MyTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Model
{
    public class ToDoInfo
    {
        private ToDoStatusEnum _Status = ToDoStatusEnum.Pending;

        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;
        public ToDoStatusEnum Status
        {
            get => _Status; set
            {
                var _ = _Status;
                _Status = value;
                if (Id != null && _ != value) this.DBUpdate();
            }
        }
    }
    public enum ToDoStatusEnum
    {
        Pending,
        InProgress,
        Completed
    }
}
