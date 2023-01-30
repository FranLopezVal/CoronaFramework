
using Corona.Systems;
using System;
using UnityEngine;
using Corona.Delegates;



namespace Corona
{

    public enum CoronaOptions
    {
        dataPath = 0,
        tempDataPath =1,
        CoronaDataPath = 2,
        saveGame = 3,
        iconsPath = 4
    }
    /// <summary>
    /// Clase principal de corona necesaria para manejar el Plug-in.
    /// Requiere UnityEngine, v2021 o superior.
    /// Desarrollada en .NET 6
    /// </summary>
    public static class Corona
    {
        private static List<c_System> _systems;
        private static Dictionary<CoronaOptions,string> _options;
        private static Dictionary<string, object> _str_options;


        private static EventWithEventHandler? OnInit;
        private static EventWithEventHandler? OnUpdate;

        /// <summary>
        /// Inicializa Corona.
        /// </summary>
        public static void Initialize(string NameGame)
        {
            //INIT VARS.
            _systems = new List<c_System>();
            _options = new Dictionary<CoronaOptions,string>();
            _str_options = new Dictionary<string, object>();

            //DEFAULT OPTIONS.
            Set(CoronaOptions.dataPath, Path.Combine(Application.dataPath, "//"));
            Set(CoronaOptions.CoronaDataPath, Path.Combine(Application.dataPath, "/Corona/"));
            Set(CoronaOptions.tempDataPath, Path.Combine(Application.dataPath, "/Corona/Temp/"));
            Set(CoronaOptions.saveGame, Path.Combine(Application.dataPath, "/Corona/Save/" + NameGame));
            Set(CoronaOptions.iconsPath, Path.Combine(Application.persistentDataPath, "/Resources/Icons/"));

            Set("Corona::InventorySystem", true);
            Set("Corona::QuestSystem", true);

            //SYSTEMS TO MANAGEMENT.
            AddSystem<c_InventorySystem>();
            AddSystem<c_CharacterSystem>();
            AddSystem<c_QuestSystem>();

            OnInit?.Invoke(new c_SystemEventArg(null,null));
        }

        public static void Update()
        {
            foreach (var sys in _systems)
            {
                if (sys.IsActive()) sys.Update(new c_SystemEventArg(null, new object[]
                {
                    Time.deltaTime
                }));
            }
            OnUpdate?.Invoke(new c_SystemEventArg(null,
                new object[]
                {
                    
                }));
        }

        /// <summary>
        /// Añade una funcion que se ejecutara al inicio de Corona, pero despues
        /// de su inicializacion.
        /// </summary>
        /// <param name="f"></param>   
        public static void AddToOnInit(EventWithEventHandler f)
        {
            OnInit += f;
        }
       
        /// <summary>
        /// Añade una funcion que se ejecutara al finalizar el Update de Corona,
        /// despues de la ejecucion de todos los sitemas.
        /// </summary>
        /// <param name="f"></param>
        public static void AddToOnUpdate(EventWithEventHandler f)
        {
            OnUpdate += f;
        }

        /// <summary>
        /// Hace una busqueda por Tipo y devuelve el sistema indicado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? FindSystem<T>() where T : c_System
        {
            foreach (var s in _systems)
            {
                if (s.GetType() is T) return (T)s;
            }
            return null;
        }

        /// <summary>
        /// Añade un sistema al Administrador de recursos de Corona.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void AddSystem<T>() where T : c_System, new()
        {
            T obj = new T();
            if(_systems.Contains(obj))
            {
                RemoveSystem<T>();
            }
            _systems.Add(obj);
        }

        /// <summary>
        /// Quita un sistema del Administrador de recursos de Corona.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RemoveSystem<T>() where T : c_System
        {
            T? item = FindSystem<T>();
            if(item!=null)_systems.Remove(item);
        }
    
        public static string? Get(CoronaOptions co)
        {
            if (_options.ContainsKey(co))
                return _options[co];
            else return String.Empty;
        }

        public static object? Get(string arg)
        {
            if (_str_options.ContainsKey(arg))
                return _str_options[arg];
            else return String.Empty;
        }
       
        /// <summary>
        /// Get devuelve los datos guardados por Corona o el usuario con el comando
        /// <see cref="Set(string, object)"/>
        /// <see cref="Set(CoronaOptions, string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T? Get<T>(string arg)
        {
            if (_str_options.ContainsKey(arg))
            {
                if(_str_options[arg].GetType() is T)
                return (T)_str_options[arg];
            }
            else return default(T);
            return default(T);
        }

        [Obsolete(message:"Esta funcion esta en desuso, utilize los argumentos de tipo: CoronaOptions")]
        public static void Set(int co,string value)
        {
            Set((CoronaOptions)co,value);
        }
      
        /// <summary>
        /// Set guarda una "variable" a modo de dato en memoria 
        /// para usar con Corona o el propio usuario posteriormente, asi 
        /// como datos y opciones.
        /// </summary>
        /// <param name="co"></param>
        /// <param name="value"></param>
        public static void Set(string arg,object value)
        {
            if (_str_options.ContainsKey(arg))
                _str_options[arg] = value;
            else _str_options.Add(arg, value);
        }
        public static void Set(CoronaOptions co, string value)
        {
            if(_options.ContainsKey(co))
            {
                _options[co] = value;
            } else _options.Add(co,value);
        }
    }
}