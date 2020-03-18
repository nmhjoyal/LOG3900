using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class CreateRoom
    {
        public string id;
        public Boolean isPrivate;
        public CreateRoom(string pid, Boolean pisPrivate)
        {
            id = pid;
            isPrivate = pisPrivate;
        }
    }

}
