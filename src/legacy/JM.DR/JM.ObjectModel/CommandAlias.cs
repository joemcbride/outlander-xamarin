using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JM.DR.ObjectModel
{
    [Serializable]
    public class CommandAlias
    {
        #region Private Members
        private String m_Alias;
        private String m_Action;
        #endregion

        #region Constructors
        public CommandAlias()
            : this(String.Empty, String.Empty)
        { }

        public CommandAlias(String alias, String action)
        {
            m_Alias = alias;
            m_Action = action;
        }
        #endregion

        #region Public Properties
        public String Alias
        {
            get
            {
                return m_Alias;
            }
            set
            {
                m_Alias = value;
            }
        }

        public String Action
        {
            get
            {
                return m_Action;
            }
            set
            {
                m_Action = value;
            }
        }
        #endregion

        #region Public Methods
        public Boolean IsMatch(String command)
        {
            return false;
        }

        public String Format(params String[] args)
        {
            if (args == null || args.Length == 0)
            {
                return m_Action;
            }

            StringBuilder sb = new StringBuilder(m_Action);

            for (Int32 i = 0; i < args.Length; i++)
            {
                sb.Replace("$" + i.ToString(), args[i]);
            }

            return sb.ToString();
        }
        #endregion
    }
}
