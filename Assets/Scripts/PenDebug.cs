using UnityEngine;

public static class PenDebug
{
    private static float NormalizeAngle(float deg)
    {
        deg %= 360f;
        if (deg < 0f)
            deg += 360f;

        return deg;
    }

    public static void LogHit(
        string plateName,
        float penetrationMm,
        float plateEffectiveArmour,
        float plateNormalDeg,
        float incomingDeg,
        float impactDeg,
        float effectiveArmourMm,
        bool ricochet,
        bool penetrated)
    {
        plateNormalDeg = NormalizeAngle(plateNormalDeg);
        incomingDeg = NormalizeAngle(incomingDeg);

        string result;

        if (ricochet)
            result = "RICOCHET";
        else if (penetrated)
            result = "PENETRATION";
        else
            result = "NO PEN";

        Debug.Log(
            $"[HIT] Plate={plateName} | " +
            $"Pen={penetrationMm:0.#}mm | " +
            $"PlateEffArm={plateEffectiveArmour:0.#}mm | " +
            $"PlateNormal={plateNormalDeg:0.#}° | " +
            $"Incoming={incomingDeg:0.#}° | " +
            $"Impact={impactDeg:0.#}° | " +
            $"EffArmor={effectiveArmourMm:0.#}mm | " +
            $"Result={result}"
        );
    }
}