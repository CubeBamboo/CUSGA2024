using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public interface IHurtable
    {
        public void OnHurt(int attackPoint);
    }
}
