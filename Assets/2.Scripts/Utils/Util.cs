using UnityEngine;

public static class Util
{
    public static Quaternion GetTargetRotation(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        angle = -angle;

        if (angle < -180)
        {
            angle += 360f;
        }

        return Quaternion.Euler(new Vector3(0, 0, angle));
    }
}