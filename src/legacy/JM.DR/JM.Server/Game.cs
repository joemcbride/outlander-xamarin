using System;
using System.Collections.Generic;
using System.Text;

namespace JM.Server
{
    public sealed class Game
    {
        private String m_Code;
        private String m_Name;

        public Game()
            : this(String.Empty, String.Empty)
        { }

        public Game(String code, String name)
        {
            m_Code = code;
            m_Name = name;
        }
        
        public String Code
        {
            get
            {
                return m_Code;
            }
            set
            {
                m_Code = value;
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
