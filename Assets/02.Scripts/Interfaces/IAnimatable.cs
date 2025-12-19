public interface IAnimatable
{
    void PlayAnimation(string stateName);
    void SetAnimationSpeed(float speed);
    void SetFloat(string paramName, float value);
    void SetBool(string paramName, bool value);
    void SetTrigger(string triggerName);
}
