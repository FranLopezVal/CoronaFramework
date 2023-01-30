using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona.Systems
{

    /// <summary>
    /// Interfaz que de termina las funciones minimas que contiene 
    /// un sistema en Corona.
    /// </summary>
    public interface c_ISystem
    {
        void Init(c_SystemEventArg? e);
        void Update(c_SystemEventArg? e);
        bool IsActive();
    }
    /// <summary>
    /// Clase base para cualquier sistema que se pueda añadir a Corona.
    /// </summary>
    public class c_System : c_ISystem
    {

        public virtual void Init(c_SystemEventArg? e)
        {}

        public virtual void Update(c_SystemEventArg? e)
        {}

        public virtual bool IsActive() { return true; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if(obj.GetType() != typeof(c_System)) return false;
            else
            return this == obj;
        }


        public static bool operator ==(c_System _s, c_System s_)
        {
            return (_s?.GetType() == s_?.GetType());
        }
        public static bool operator !=(c_System? _s, c_System? s_)
        {
            return _s?.GetType() == s_?.GetType();
        }
    }
}
