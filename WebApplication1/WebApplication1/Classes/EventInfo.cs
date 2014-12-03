using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Classes
{
    public class EventInfo
    {
        public int id;
        public string start;
        public string end;
        public int eng_id = 0;
        public string status;
        public string customer;
        public string number;
        public string category;

        //Default Constructor, all to null except id
        public EventInfo()
        {
            id = 0;
            start = null;
            end = null;
            eng_id = 0;
            status = null;
            customer = null;
            number = null;
            category = null;
        }

        //Parameter constructor
        public EventInfo(int id, string s, string e, int e_id, string st, string c, string n, string ca)
        {
            this.id = id;
            start = s;
            end = e;
            eng_id = e_id;
            status = st;
            customer = c;
            number = n;
            category = ca;
        }

        public void setID(int id)
        {
            this.id = id;
        }
    }
}