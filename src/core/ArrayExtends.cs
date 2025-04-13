using System.Runtime.CompilerServices;
using GDC = Godot.Collections;

namespace TinyConverter.Core;

public static partial class ArrayExtens
{
    /// <summary>
    /// Assigns elements of another array into the array. 
    /// Resizes the array to match array. 
    /// Performs type conversions if the array is typed.
    /// </summary>
    /// <param name="p_item">Element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assign<[MustBeVariant] T>(this GDC::Array<T> p_arr, GDC::Array<T> p_item)
    {
        p_arr.Clear();
        p_arr.AddRange(p_item);
    }

    /// <summary>
    /// Returns the last element of the array. 
    /// Prints an error and returns null if the array is empty.
    /// Note: Calling this function is not the same as writing array[-1]. 
    /// If the array is empty, accessing by index will pause project execution when running from the editor.
    /// </summary>
    /// <returns>The last element of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Back<[MustBeVariant] T>(this GDC::Array<T> p_arr) => p_arr[^1];

    /// <summary>
    /// Returns the first element of the array. 
    /// Prints an error and returns null if the array is empty.
    /// Note: Calling this function is not the same as writing array[0]. 
    /// If the array is empty, accessing by index will pause project execution when running from the editor.
    /// </summary>
    /// <returns>The first element of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Front<[MustBeVariant] T>(this GDC::Array<T> p_arr)
    {
        return p_arr[0];
    }

    /// <summary>
    /// Removes and returns the element of the array at index position. 
    /// If negative, position is considered relative to the end of the array. 
    /// Leaves the array untouched and returns null if the array is empty or if it's accessed out of bounds. 
    /// An error message is printed when the array is accessed out of bounds, but not when the array is empty.
    /// Note: On large arrays, this method can be slower than pop_back as it will reindex the array's elements 
    /// that are located after the removed element. 
    /// The larger the array and the lower the index of the removed element, the slower pop_at will be.
    /// </summary>
    /// <param name="position">Index position.</param>
    /// <returns>The element of the array at index position.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PopAt<[MustBeVariant] T>(this GDC::Array<T> p_arr, int p_position)
    {
        T element = p_arr[p_position];
        p_arr.RemoveAt(p_position);
        return element;
    }

    /// <summary>
    /// Adds an element at the beginning of the array. See also push_back.
    /// Note: On large arrays, this method is much slower than push_back as it will reindex all the array's elements every time it's called. 
    /// The larger the array, the slower push_front will be.
    /// </summary>
    /// <param name="item">Element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PushFront<[MustBeVariant] T>(this GDC::Array<T> p_arr, T p_item)
    {
        p_arr.Insert(0, p_item);
    }

    /// <summary>
    /// Appends an element at the end of the array. See also push_front.
    /// </summary>
    /// <param name="item">Element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PushBack<[MustBeVariant] T>(this GDC::Array<T> p_arr, T p_item)
    {
        p_arr.Insert(p_arr.Count, p_item);
    }

    /// <summary>
    /// Removes and returns the first element of the array. 
    /// Returns null if the array is empty, without printing an error message. See also pop_back.
    /// Note: On large arrays, 
    /// this method is much slower than pop_back as it will reindex all the array's elements every time 
    /// it's called. The larger the array, the slower pop_front will be.
    /// </summary>
    /// <returns>The first element of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PopFront<[MustBeVariant] T>(this GDC::Array<T> p_arr)
    {
        T firstElement = p_arr[0];
        p_arr.RemoveAt(0);
        return firstElement;
    }

    /// <summary>
    /// Removes and returns the last element of the array. 
    /// Returns null if the array is empty, without printing an error message. 
    /// See also pop_front.
    /// </summary>
    /// <returns>The last element of the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PopBack<[MustBeVariant] T>(this GDC::Array<T> p_arr)
    {
        T element = p_arr[^1];
        p_arr.RemoveAt(p_arr.Count - 1);
        return element;
    }

    /// <summary>
    /// Executes a provided function once for each array element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<[MustBeVariant] T>(this GDC::Array<T> p_items, System.Action<T> p_action)
    {
        foreach (T item in p_items)
            p_action(item);
    }
}