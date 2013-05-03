using System;
using System.Collections.Generic;
using System.Text;

namespace JM.DR.ObjectModel
{
    public class Character
    {
        private String m_UserID;
        private String m_Name;

        public Character()
            : this(String.Empty, String.Empty)
        { }

        public Character(String name, String userID)
        {
            m_Name = name;
            m_UserID = userID;
        }

        public String UserID
        {
            get
            {
                return m_UserID;
            }
            set
            {
                m_UserID = value;
            }
        }
        public String Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
    }
}
