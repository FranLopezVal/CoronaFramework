using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Corona
{
    public abstract class c_Quest : MonoBehaviour
    {
        public int Id;
        public string Name;
        public string Definition;

        protected bool _isCompleted=false;
        

        /// <summary>
        /// Las etapas deben ser referenciadas a la hora de crear la mision.
        /// </summary>
        public List<c_QuestStage> Stages;

        public virtual void OnAccept()
        {
            if (Stages == null) Stages = new List<c_QuestStage>();
        }
        public virtual void OnUpdate()
        {

        }
        public abstract void OnStageUpdate();
        public abstract void OnComplete();

        public bool IsCompleted => _isCompleted;

        public void CompleteCurrentStage()
        {

        }

    }
}
/*
 * Ques stage tendra unna variable de objetivos completados.
 * para cambiar de stage.
 */
