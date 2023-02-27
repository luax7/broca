
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vec2Interpreter : MonoBehaviour
{
    public List<Vector2> List = new List<Vector2>();
    public List<TMPro.TMP_InputField> Inputs;
    public Oracle Oracle;

    public void Run()
    {
        List<Vector2> list = new List<Vector2>();

        for (int Index = 1; Index < Inputs.Count || Inputs.Count % 2 != 0; Index += 2)
        {
            int Val1 = int.Parse(Inputs[Index-1].text);
            int Val2 = int.Parse(Inputs[Index].text);

            Vector2 output = new Vector2(Val2,Val1);

            list.Add(output);
        }

        List = list;

        Oracle.Points = List;
    }
}
