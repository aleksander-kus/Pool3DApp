using DomainLayer;
using DomainLayer.Dto;
using System;
using System.Drawing;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public class IlluminationService
    {
        private readonly IlluminationParameters parameters;

        public IlluminationService(IlluminationParameters parameters)
        {
            this.parameters = parameters;
        }

        public Color ComputeColor(Vector3 point, Vector3 normalVector, Color color, Camera camera)
        {
            // ambient
            var baseColor = color.From255() * parameters.Ka;

            //// the base color of point
            //var I_O = parameters.ColoringMode == ColoringMode.SolidColor ? parameters.SceneColor.From255() : GetColorFromTexture(point, parameters).From255();
            //// light color
            var I_L = Vector3.One;
            var N = Vector3.Normalize(normalVector);
            //// additional versors
            var V = Vector3.Normalize(camera.Position.Coordinates - point);// / Vector3.DistanceSquared(camera.Position.Coordinates, point);
            var sourceLocation = parameters.MainLightPosition.Coordinates;
            var L = Vector3.Normalize(sourceLocation - point);

            var sourceDistance = Vector3.DistanceSquared(sourceLocation, point);
            I_L /= sourceDistance;
            var R = 2 * Vector3.Dot(N, L) * N - L;
            baseColor += I_L * parameters.Kd * CosineBetweenVectors(N, L) + I_L * parameters.Ks * (float)Math.Pow(CosineBetweenVectors(V, R), parameters.N);
            //var actualColor2 = I_L * I_O * parameters.Ks * (float)Math.Pow(CosineBetweenVectors(V, R), parameters.N);
            //if (parameters.LightMode == LightMode.Normal)
            //    return (actualColor1 + actualColor2).To255();
            //(Vector3 position, Color color) reflector1 = (new Vector3(0, parameters.CanvasY, parameters.Radius + parameters.H), Color.Red);
            //(Vector3 position, Color color) reflector2 = (new Vector3(parameters.CanvasX, parameters.CanvasY, parameters.Radius + parameters.H), Color.Green);
            //(Vector3 position, Color color) reflector3 = (new Vector3(parameters.CanvasX / 2, 0, parameters.Radius + parameters.H), Color.Blue);
            //var ret = CalculateColorFromLight(reflector1.position, reflector1.color, point, N, I_O, parameters) +
            //    CalculateColorFromLight(reflector2.position, reflector2.color, point, N, I_O, parameters) + CalculateColorFromLight(reflector3.position, reflector3.color, point, N, I_O, parameters)
            //    + actualColor1 + actualColor2;
            //normalVector += Vector3.One;
            //normalVector *= 1 / 2f;
            //normalVector = Vector3.Normalize(normalVector);
            //baseColor = baseColor * 1 / Vector3.Distance(camera.Position.Coordinates, point);
            return baseColor.To255();
            //return normalVector.To255();
        }

        private static float CosineBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            var ret = vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
            return Math.Max(ret, 0);
        }
    }
}
