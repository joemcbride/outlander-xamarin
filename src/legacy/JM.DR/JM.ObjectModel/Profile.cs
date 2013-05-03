using System;
using System.Collections.Generic;

namespace JM.DR.ObjectModel
{
    public class Profile
    {
        private String m_Name;

        public Profile(String name)
        {
            m_Name = name;
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
