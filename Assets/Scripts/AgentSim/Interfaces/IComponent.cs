using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent 
{
    string componentName { get; }

    string state { get; set; }

    bool bound { get; }

    bool Matches (IComponent other);
}
