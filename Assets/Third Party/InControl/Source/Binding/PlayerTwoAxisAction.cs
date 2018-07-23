using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace InControl
{
	public class PlayerTwoAxisAction : TwoAxisInputControl
	{
		PlayerAction negativeXAction;
		PlayerAction positiveXAction;
		PlayerAction negativeYAction;
		PlayerAction positiveYAction;

        string horizontalAxis;
        string verticalAxis;

		/// <summary>
		/// Gets or sets a value indicating whether the Y axis should be inverted for
		/// this action. When false (default), the Y axis will be positive up,
		/// the same as Unity.
		/// </summary>
		public bool InvertYAxis { get; set; }

		internal PlayerTwoAxisAction( PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction )
		{
			this.negativeXAction = negativeXAction;
			this.positiveXAction = positiveXAction;
			this.negativeYAction = negativeYAction;
			this.positiveYAction = positiveYAction;

			InvertYAxis = false;
			Raw = true;
		}

        internal PlayerTwoAxisAction( string horizontal, string vertical )
        {
            horizontalAxis = horizontal;
            verticalAxis = vertical;

            InvertYAxis = false;
            Raw = true;
        }


		internal void Update( ulong updateTick, float deltaTime )
		{
            if (horizontalAxis == null && verticalAxis == null)
            {
                var x = ValueFromSides(negativeXAction, positiveXAction, false);
                var y = ValueFromSides(negativeYAction, positiveYAction, InputManager.InvertYAxis || InvertYAxis);

                UpdateWithAxes(x, y, updateTick, deltaTime);
            }
            else
            {
                //Debug.Log(Input.GetAxis(horizontalAxis));
                //Debug.Log(CrossPlatformInputManager.AxisExists(horizontalAxis));
                //Debug.Log(CrossPlatformInputManager.GetAxis(horizontalAxis));
                UpdateWithAxes(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis), updateTick, deltaTime);
            }
		}


		float ValueFromSides( float negativeSideValue, float positiveSideValue, bool invertSides )
		{
			var nsv = Utility.Abs( negativeSideValue );
			var psv = Utility.Abs( positiveSideValue );

			if (Utility.Approximately( nsv, psv ))
			{
				return 0.0f;
			}

			if (invertSides)
			{
				return nsv > psv ? nsv : -psv;
			}
			else
			{
				return nsv > psv ? -nsv : psv;
			}
		}


	}
}
