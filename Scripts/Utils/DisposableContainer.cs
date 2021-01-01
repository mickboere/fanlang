using System;
using System.Collections.Generic;

namespace FanLang
{
	/// <summary>
	/// Container object which allows for easy disposing of multiple <see cref="IDisposable"/>s.
	/// </summary>
	public class DisposableContainer
	{
		private List<IDisposable> disposables;

		public DisposableContainer()
		{
			disposables = new List<IDisposable>();
		}

		public DisposableContainer(List<IDisposable> disposables)
		{
			this.disposables = disposables;
		}

		/// <summary>
		/// Adds new <see cref="IDisposable"/> to the container.
		/// </summary>
		/// <param name="disposable"></param>
		public void Add(IDisposable disposable)
		{
			disposables.Add(disposable);
		}

		/// <summary>
		/// Removes the <see cref="IDisposable"/> from the container but doesn't dispose it.
		/// </summary>
		/// <param name="disposable"></param>
		public void Remove(IDisposable disposable)
		{
			disposables.Remove(disposable);
		}

		/// <summary>
		/// Disposes of and removes <see cref="IDisposable"/> from container.
		/// </summary>
		public void Dispose(IDisposable disposable)
		{
			Remove(disposable);
			disposable.Dispose();
		}

		/// <summary>
		/// Disposes of all the stored <see cref="IDisposable"/>s and clears the container.
		/// The container itself is not disposed of.
		/// </summary>
		public void Dispose()
		{
			foreach (IDisposable disposable in disposables)
			{
				disposable.Dispose();
			}
			disposables.Clear();
		}
	}
}
