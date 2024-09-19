using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcuTool
{
    public class RegistryItem
    {
        const string DEBUG_KEY_PATH = "Software\\Oculus\\RemoteHeadset";
        RegistryKey DebugKey = Registry.CurrentUser.OpenSubKey(DEBUG_KEY_PATH, true);

        string  valueName;
        string valueType;
        int value;

        public RegistryItem(string valueName, string valueType)
        {
            this.valueName = valueName;
            this.valueType = valueType;

            getRegistryValue();
        }

        public int getRegistryValue()   // Update the objects value field to whatever is set in the registry, then return
        {
            var tempValue = DebugKey.GetValue(valueName);

            if (tempValue == null)
            {
                value = -1;
            }
            else
            {
                value = Convert.ToInt32(DebugKey.GetValue(valueName));
            }
            return value;
        }

        public int getObjectValue() // Just return the objects value without changing it
        {
            return value;
        }

        public void setValue(int value) // -1 deletes value, otherwise set it
        {
            if (value == -1)
            {
                if (DebugKey.GetValue(valueName) == null)
                {
                    return;
                }
                DebugKey.DeleteValue(valueName);
            }
            else
            {
                this.value = value;
                DebugKey.SetValue(valueName, value);
            }
        }
    }
}
