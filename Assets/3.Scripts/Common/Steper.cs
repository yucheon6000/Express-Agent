using System;

public class Stepper<T>
{
    private T[] steps;

    public Stepper(T[] steps)
    {
        if (steps == null || steps.Length == 0)
            throw new System.Exception("steps 배열이 없거나, 비었습니다.");

        this.steps = steps;
    }

    public T GetStep(int step)
    {
        int index = Math.Clamp(step, 0, steps.Length - 1);
        return steps[index];
    }
}