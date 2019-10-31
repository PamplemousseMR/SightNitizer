using System.Collections.Generic;

namespace SightProperties
{
    class Logs
    {
        private Logs()
        {
            m_errors = new List<string>();
            m_info = new List<string>();
        }

        static private Logs s_instance = new Logs();

        static public Logs getInstance()
        {
            return s_instance;
        }

        public void error(string _message)
        {
            m_errors.Add(_message);
        }

        public void info(string _message)
        {
            m_info.Add(_message);
        }

        public List<string> getErrors()
        {
            return m_errors;
        }

        public List<string> getInfo()
        {
            return m_info;
        }

        private List<string> m_errors;
        private List<string> m_info;
    }
}
