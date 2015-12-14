using System;
using System.Collections.Generic;
using nginz.Common;
using OpenTK.Input;
using System.Linq;

namespace nginz
{

	/// <summary>
	/// Keyboard buffer.
	/// </summary>
	public class KeyboardBuffer : ICanLog
	{

		/// <summary>
		/// The keys.
		/// </summary>
		readonly List<KeyState> Keys;

		readonly public Dictionary<Key, char> Alphanumeric =
			new Dictionary<Key, char> {
			{ Key.A, 'A' }, { Key.B, 'B' },
			{ Key.C, 'C' }, { Key.D, 'D' },
			{ Key.E, 'E' }, { Key.F, 'F' },
			{ Key.G, 'G' }, { Key.H, 'H' },
			{ Key.I, 'I' }, { Key.J, 'J' },
			{ Key.K, 'K' }, { Key.L, 'L' },
			{ Key.M, 'M' }, { Key.N, 'N' },
			{ Key.O, 'O' }, { Key.P, 'P' },
			{ Key.Q, 'Q' }, { Key.R, 'R' },
			{ Key.S, 'S' }, { Key.T, 'T' },
			{ Key.U, 'U' }, { Key.V, 'V' },
			{ Key.W, 'W' }, { Key.X, 'X' },
			{ Key.Y, 'Y' }, { Key.Z, 'Z' },
			{ Key.Number0, '0' }, { Key.Number1, '1' },
			{ Key.Number2, '2' }, { Key.Number3, '3' },
			{ Key.Number4, '4' }, { Key.Number5, '5' },
			{ Key.Number6, '6' }, { Key.Number7, '7' },
			{ Key.Number8, '8' }, { Key.Number9, '9' },
			{ Key.Space, ' ' },
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.KeyboardBuffer"/> class.
		/// </summary>
		public KeyboardBuffer () {
			Keys = new List<KeyState> ();
		}

		public bool IsAlphanumericKey (Key k) {
			return Alphanumeric.ContainsKey (k);
		}

		public char GetKeyChar (Key k) {
			return Alphanumeric [k];
		}

		/// <summary>
		/// Determines whether the specified key is currently pressed.
		/// </summary>
		/// <returns>Whether the specified key is currently pressed.</returns>
		/// <param name="key">Key.</param>
		public bool IsKeyDown (Key key) {

			// Register the key
			RegisterKey (key);

			// Return whether the key is currently pressed.
			return Keys.First (state => key == state.Key).IsDown;
		}

		/// <summary>
		/// Determines whether any of the specified keys is currently pressed.
		/// </summary>
		/// <returns>Whether any of the specified keys is currently pressed.</returns>
		/// <param name="keys">Keys.</param>
		public bool IsAnyKeyDown (params Key[] keys) {

			// Iterate over the keys
			foreach (var key in keys) {

				// Return true if the key is currently pressed.
				if (IsKeyDown (key))
					return true;
			}

			// Return false if none of the specified keys is currently pressed.
			return false;
		}

		/// <summary>
		/// Determines whether the specified key is currently released.
		/// </summary>
		/// <returns>Whether the specified key is currently released.</returns>
		/// <param name="key">Key.</param>
		public bool IsKeyUp (Key key) {

			// Return whether the key is currently released.
			return !IsKeyDown (key);
		}

		/// <summary>
		/// Determines whether any of the specified keys is currently released.
		/// </summary>
		/// <returns>Whether any of the specified keys is currently released.</returns>
		/// <param name="keys">Keys.</param>
		public bool IsAnyKeyUp (params Key[] keys) {

			// Return whether any of the keys is currently released
			return !IsAnyKeyDown (keys);
		}

		public bool IsKeyTyped (Key key) {

			// Check if the key is currently pressed
			if (IsKeyDown (key)) {

				// Return false if the key is already frozen
				if (Keys.First (state => key == state.Key).IsFrozen)
					return false;

				// Freeze the key
				Keys.First (state => key == state.Key).IsFrozen = true;

				// Return true
				return true;
			}

			// Return false
			return false;
		}

		public bool IsAnyKeyTyped (params Key[] keys) {

			// Return true if any of the specified key is currently pressed
			foreach (var key in keys)
				if (IsKeyTyped (key))
					return true;

			// Return false
			return false;
		}

		public bool IsAnyAlphanumericKeyTyped () {
			foreach (var kvp in Alphanumeric)
				if (IsKeyDown (kvp.Key))
					return true;
			return false;
		}

		/// <summary>
		/// KeyDown event handler.
		/// </summary>
		/// <param name="dummy">Dummy.</param>
		/// <param name="e">EventArgs.</param>
		internal void RegisterKeyDown (object dummy, KeyboardKeyEventArgs e) {
			
			// Register the key
			RegisterKey (e.Key);

			// Set the key down flag to true
			Keys.First (state => e.Key == state.Key).IsDown = true;
		}

		/// <summary>
		/// KeyUp event handler.
		/// </summary>
		/// <param name="dummy">Dummy.</param>
		/// <param name="e">EventArgs.</param>
		internal void RegisterKeyUp (object dummy, KeyboardKeyEventArgs e) {
			
			// Register the key
			RegisterKey (e.Key);

			// Unfreeze the key
			Keys.First (state => e.Key == state.Key).IsFrozen = false;

			// Set the key down flag to false
			Keys.First (state => e.Key == state.Key).IsDown = false;
		}

		/// <summary>
		/// Register a key.
		/// </summary>
		/// <param name="key">Key.</param>
		void RegisterKey (Key key) {

			// Add the key to the buffer if it isn't yet there
			if (Keys.All (state => key != state.Key))
				Keys.Add (KeyState.FromKey (key));
		}
	}
}

