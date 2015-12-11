using System;

namespace nginz.Compatibility.Iodine
{
	/// <summary>
	/// Content pipeline.
	/// </summary>
	public class Pipeline
	{
		/// <summary>
		/// The game.
		/// </summary>
		readonly Game Game;

		/// <summary>
		/// Initializes a new instance of the <see cref="nginz.Compatibility.Iodine.Pipeline"/> class.
		/// </summary>
		/// <param name="game">Game.</param>
		public Pipeline (Game game) {
			Game = game;
		}

		/// <summary>
		/// Set the content root.
		/// </summary>
		/// <param name="root">Root.</param>
		public void SetRoot (string root) {
			Game.Content.ContentRoot = root;
		}

		/// <summary>
		/// Load a <see cref="Texture2D"/>.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="path">Path.</param>
		public Texture2D LoadTexture2D (string path) {
			return Game.Content.Load<Texture2D> (path);
		}

		/// <summary>
		/// Load a <see cref="Texture2D"/>. 
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="path">Path.</param>
		/// <param name="conf">Conf.</param>
		public Texture2D LoadTexture2D (string path, TextureConfiguration conf) {
			return Game.Content.Load<Texture2D> (path, conf);
		}
	}
}

