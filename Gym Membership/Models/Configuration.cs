using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Configuration
    {
        public string ConfigurationKey { get; set; }
        public string ConfigurationValue { get; set; }
        
        public string ConfigurationDescription { get; set; }
        public string ConfigurationType { get; set; }

        public float ConfigurationValueFloat
        {
            get
            {
                    float result = 0f;
                if (ConfigurationType=="float")
                {
                     float.TryParse(ConfigurationValue, out  result);

                }
                    return result;
            }

        }

        public int ConfigurationValueInt
        {
            get
            {
                int result = 0;
                if (ConfigurationType == "int")
                {
                    int.TryParse(ConfigurationValue, out result);

                }
                return result;
            }

        }
    }
}