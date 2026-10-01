using UnityEngine;

public interface ISkillLogic
{
    void Initialize(SkillInstance instance);
    void Activate(Transform target);
    void Dispose();
}