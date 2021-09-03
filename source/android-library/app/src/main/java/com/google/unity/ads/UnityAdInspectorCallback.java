package com.google.unity.ads;

import com.google.android.gms.ads.AdError;

/**
 * An interface form of {@link UnityAdInspectorCallback} that can be implemented via {@code
 * AndroidJavaProxy} in Unity to receive ad events synchronously.
 */
public interface UnityAdInspectorCallback {

  void onAdInspectorError(AdError error);
}


