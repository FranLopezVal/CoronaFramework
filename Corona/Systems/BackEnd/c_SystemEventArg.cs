using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using GO = UnityEngine.GameObject;

namespace Corona.Systems
{
    /// <summary>
    /// Clase que controla los argumentos de los eventos
    /// genericos que se lanzan cuando se ejecuta una funcion de
    /// evento de cualquier sistema en Corona.
    /// </summary>
    public class c_SystemEventArg : EventArgs
    {
        GO _sender;
        object[] _args;
        int _args_lenght;


        /// <summary>
        /// Inicializacion de la clase, donde sender es el <see cref="UnityEngine.GameObject"/>
        /// que manda el evento, puede ser <see cref="Nullable"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public c_SystemEventArg(GO? sender,object[] args)
        {
            _sender = sender !=null ? sender : new GO("sys_event_arg");
            _args = args; _args_lenght= args.Length;
        }

        public GO GetSender => _sender;

        public bool IsSysEventArg => _sender.name == "sys_event_arg";

        public object GetArg(int index)
        {
            if (index >= 0 && index < _args_lenght)
                return _args[index];
            else _ = string.Empty;
            return string.Empty;
        }
    }

}
