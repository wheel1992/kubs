using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class KubsDebug
    {
        private bool _isDebug = false;
        public KubsDebug(bool isDebug)
        {
            _isDebug = isDebug;
        }
        public void Log(object s)
        {
            if (_isDebug)
            {
                Debug.Log(s);
            }
        }
    }
}

