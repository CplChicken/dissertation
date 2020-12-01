using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class playerAcademy : Academy
{
    private gameArea[] gameAreas;

    public override void AcademyReset() {
        // Get all game areas
        if (gameAreas == null) {
            gameAreas = FindObjectsOfType<gameArea>();
        }

        // Set up Areas
        foreach (gameArea gameArea in gameAreas) {
            gameArea.groundSpeed = resetParameters["ground_speed"];
            gameArea.difficulty = resetParameters["difficulty"];
            gameArea.ResetArea();
        }
    }
}
