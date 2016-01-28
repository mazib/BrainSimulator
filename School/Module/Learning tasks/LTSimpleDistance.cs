﻿using GoodAI.Modules.School.Common;
using System;
using System.Drawing;

namespace GoodAI.Modules.School.LearningTasks
{
    public class LTSimpleDistance : AbstractLearningTask<ManInWorld>
    {
        public const string COLOR_PATTERNS = "Color patterns";
        public const string ERROR_TOLERANCE = "Target distance levels";

        private Random m_rndGen = new Random();
        private GameObject m_agent;
        private GameObject m_target;
        private float m_distance = 0; // ranging from 0 to 1; 0-0.125 is smallest, 0.875-1 is biggest; m_distance is lower bound of the interval

        public LTSimpleDistance() { }

        public LTSimpleDistance(ManInWorld w)
            : base(w)
        {

            TSHints = new TrainingSetHints
            {
                { COLOR_PATTERNS, 0 },
                { TSHintAttributes.VARIABLE_SIZE, 0 },
                { ERROR_TOLERANCE, 0.25f },
                { TSHintAttributes.TARGET_IMAGE_VARIABILITY, 1 },
                { TSHintAttributes.NOISE, 0 },
                { TSHintAttributes.GIVE_PARTIAL_REWARDS, 1 },
                { TSHintAttributes.MAX_NUMBER_OF_ATTEMPTS, 10000 }
            };

            TSProgression.Add(TSHints.Clone());
            TSProgression.Add(new TrainingSetHints {
                { COLOR_PATTERNS, 1 },
                { ERROR_TOLERANCE, 0.15f },
                { TSHintAttributes.GIVE_PARTIAL_REWARDS, 0 }
            });
            TSProgression.Add(new TrainingSetHints {
                { TSHintAttributes.TARGET_IMAGE_VARIABILITY, 2 }
            });
            TSProgression.Add(new TrainingSetHints {
                { TSHintAttributes.VARIABLE_SIZE, 1 },
                { ERROR_TOLERANCE, 0.10f },
                { TSHintAttributes.MAX_NUMBER_OF_ATTEMPTS, 1000 }
            });
            TSProgression.Add(new TrainingSetHints {
                { ERROR_TOLERANCE, 0.05f },
                { TSHintAttributes.TARGET_IMAGE_VARIABILITY, 3 },
                { TSHintAttributes.MAX_NUMBER_OF_ATTEMPTS, 100 }
            });
            TSProgression.Add(TSHintAttributes.NOISE, 1);

            SetHints(TSHints);
        }

        protected override void PresentNewTrainingUnit()
        {
            World.FreezeWorld(true);

            World.IsImageNoise = TSHints[TSHintAttributes.NOISE] >= 1 ? true : false;

            CreateAgent();
            CreateTarget();
        }

        protected override bool DidTrainingUnitComplete(ref bool wasUnitSuccessful)
        {
            float tolerance = TSHints[ERROR_TOLERANCE];
            //Console.WriteLine(m_distance);
            //Console.WriteLine(m_distance - tolerance);
            //Console.WriteLine(m_distance + tolerance);
            // require immediate decision - in a single step
            if (m_distance - tolerance <= World.Controls.Host[0] && World.Controls.Host[0] <= m_distance + tolerance)
            {
                wasUnitSuccessful = true;
            }
            else
            {
                wasUnitSuccessful = false;
            }
            //Console.WriteLine(wasUnitSuccessful);
            // TODO: partial reward
            return true;
        }

        private void CreateAgent()
        {
            World.CreateAgent(null, 0, 0);
            m_agent = World.Agent;
            // center the agent
            m_agent.X = World.FOW_WIDTH / 2 - m_agent.Width / 2;
            m_agent.Y = World.FOW_HEIGHT / 2 - m_agent.Height / 2;
        }

        // scale and position the target:
        private void CreateTarget()
        {
            Size size;
            if (TSHints[TSHintAttributes.VARIABLE_SIZE] >= 1)
            {
                int side = m_rndGen.Next(8, 24);
                size = new Size(side, side);
            }
            else
            {
                int side = 10;
                size = new Size(side, side);
            }

            Point position = World.RandomPositionInsidePow(m_rndGen, size, true);

            Shape.Shapes shape;
            switch (m_rndGen.Next(0, (int)TSHints[TSHintAttributes.TARGET_IMAGE_VARIABILITY]))
            {
                case 0:
                default:
                    shape = Shape.Shapes.Circle;
                    break;
                case 1:
                    shape = Shape.Shapes.Square;
                    break;
                case 2:
                    shape = Shape.Shapes.Triangle;
                    break;
                case 3:
                    shape = Shape.Shapes.Mountains;
                    break;
            }

            Color color;
            if (TSHints[COLOR_PATTERNS] >= 1)
            {
                color = LearningTaskHelpers.RandomVisibleColor(m_rndGen);
            }
            else
            {
                color = Color.White;
            }

            m_target = World.CreateShape(position, shape, color, size);

            float distance = m_target.CenterDistanceTo(m_agent);
            float maxDistance = (float)Math.Sqrt(Math.Pow(World.POW_WIDTH / 2, 2) + Math.Pow(World.POW_HEIGHT / 2, 2));
            m_distance = distance / maxDistance;
        }
    }
}
