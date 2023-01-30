using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Corona;
using Corona.Systems;

namespace Corona.Delegates
{
    public delegate void EventVoid();
    public delegate void EventWithEventHandler(c_SystemEventArg ea);
}
