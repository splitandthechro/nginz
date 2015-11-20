/**
  * Copyright (c) 2015, GruntTheDivine All rights reserved.

  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  * 
  *  * Redistributions of source code must retain the above copyright notice, this list
  *    of conditions and the following disclaimer.
  * 
  *  * Redistributions in binary form must reproduce the above copyright notice, this
  *    list of conditions and the following disclaimer in the documentation and/or
  *    other materials provided with the distribution.

  * Neither the name of the copyright holder nor the names of its contributors may be
  * used to endorse or promote products derived from this software without specific
  * prior written permission.
  * 
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
  * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
  * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
  * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
  * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
  * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
  * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
  * CONTRACT ,STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
  * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
  * DAMAGE.
**/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Iodine
{
	/// <summary>
	/// Error log.
	/// </summary>
	public sealed class ErrorLog : IEnumerable <Error>
	{
		private List<Error> errors = new List<Error> ();

		/// <summary>
		/// Gets the error count.
		/// </summary>
		/// <value>The error count.</value>
		public int ErrorCount { private set; get; }

		/// <summary>
		/// Gets the warning count.
		/// </summary>
		/// <value>The warning count.</value>
		public int WarningCount { private set; get; }

		/// <summary>
		/// Gets the errors.
		/// </summary>
		/// <value>The errors.</value>
		public IList<Error> Errors {
			get {
				return errors;
			}
		}

		/// <summary>
		/// Adds the error.
		/// </summary>
		/// <param name="etype">Error type.</param>
		/// <param name="location">Error location.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void AddError (ErrorType etype, Location location, string format, params object[] args)
		{
			errors.Add (new Error (etype, location, String.Format (format, args)));
			ErrorCount++;
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator <Error> GetEnumerator ()
		{
			return errors.GetEnumerator ();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}

