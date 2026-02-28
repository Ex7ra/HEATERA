using UnityEngine;

public static class PenDebug
{
    static float NormalizeAngle(float deg)
    {
        deg %= 360f;
        if (deg < 0f) deg += 360f;
        return deg;
    }

    public static void LogHit(
        string plateName,
        float penMm,
        float plateEffArmour,
        float plateNormalDeg,
        float incomingDeg,
        float impactDeg,
        float effMm,
        bool ricochet,
        bool penetrated)
    {
        plateNormalDeg = NormalizeAngle(plateNormalDeg);
        incomingDeg = NormalizeAngle(incomingDeg);

        string result = ricochet ? "RICOCHET" : (penetrated ? "PENETRATION" : "NO PEN");
        string line =
            $"[HIT] Plate={plateName} | Pen={penMm:0.#}mm | PlateEffArm={plateEffArmour:0.#}mm | " +
            $"PlateNormal={plateNormalDeg:0.#}° | Incoming={incomingDeg:0.#}° | " +
            $"Impact={impactDeg:0.#}° | EffArmor={effMm:0.#}mm | Result={result}";

        Debug.Log(line);
    }
}
