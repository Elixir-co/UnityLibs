﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.WSA.WebCam;
//using Windows.Media.Devices;
//using System.Runtime.InteropServices;

namespace BasicExtends {

    public interface IXrCamera {
        void Capture ( Action start = null, Action end = null );
        void CameraWakeUp ( Action action );
        void PhotoMode ( Action start );
        void CameraRelease ( Action action );
        void SetHoloOpacity ( float opacity );
    }

    /// <summary>
    /// Unityのオブジェクトと一緒にWinXRのWebCam画像を取得し、
    /// </summary>
    public class CaptureCam : IXrCamera {
        private static CameraParameters mCameraParam;
        private static HardwareCameraWrapper mResolution = new HardwareCameraWrapper();
        private PhotoCapture mCameraInstance = null;
        private bool mRecMode = false;
        private bool mPhotoWakeupping = false;
        private float mOpacity = 0.9f;
        private string mTo = "";
        private const string DATA_MSG_AS = "MessageTransporter";

        public CaptureCam (string to) {
            mTo = to;
            Assert.IsNotNull(mResolution);
        }

        /// <summary>
        /// WebCameraとしてカメラ画像をキャプチャする
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Capture ( Action start = null, Action end = null ) {
            Debug.LogFormat("Capture => {0}", "call 1");
            if (mCameraInstance == null || mRecMode == false) {
                throw new Exception();
            }

            Debug.LogFormat("Capture => {0}", "call 2");

            mCameraInstance.TakePhotoAsync(( result, captured ) =>
            {
                Debug.LogFormat("Capture => {0}", result.success ? "OK" : "filed");
                if (result.success == false) { return; }
                if (start != null) { start.Invoke(); }

                List<byte> buf = new List<byte>();
                captured.CopyRawImageDataIntoBuffer(buf);

                // 撮影データのメッセージ送信
                Msg.Gen().To(mTo).As(DATA_MSG_AS)
                    .Act("Print")
                    .Set("Data",buf.Count)
                    .SetObjectData(buf).Pool();

                if (end != null) { end.Invoke(); }
            });
        }

        public void CameraWakeUp ( Action action ) {
            // カメラ操作用のインスタンスの作成
            if (mCameraInstance != null) { return; }
            if (mPhotoWakeupping == true) { return; }
            mPhotoWakeupping = true;
            Debug.LogFormat("wake up...");
            PhotoCapture.CreateAsync(true, ( PhotoCapture cameraInstance ) =>
            {
                mCameraInstance = cameraInstance;
                mPhotoWakeupping = false;

                //GCHandle campointer = (GCHandle) cameraInstance.GetUnsafePointerToVideoDeviceController();
                //VideoDeviceController vdc = campointer.Target as VideoDeviceController;
                //Debug.Log(vdc.GetType());

                Debug.LogFormat("wake up OK");
                action();
            });
        }

        public void PhotoMode ( Action start ) {
            if (mCameraInstance == null) { return; }
            if (mRecMode == true) { return; }
            Debug.LogFormat("photo mode...");
            var res = mResolution.GetResolution();
            CameraParameters c_param = 
                InitCameraParams(res, mOpacity);
            mCameraInstance.StartPhotoModeAsync(c_param, ( result ) =>
            {
                mRecMode = result.success;
                Debug.LogFormat("Start photo mode");
                if (result.success == false) {
                    Debug.LogFormat("Start photo mode => faild");
                    return;
                }

                // テクスチャセットアップのための解像度通知
                Msg.Gen().To(mTo).As(DATA_MSG_AS)
                    .Act("Setup")
                    .Set("w", res.width)
                    .Set("h", res.height).Push();
                start();
            });
        }

        public void CameraRelease ( Action action ) {
            // カメラを非アクティベート化します
            if (mCameraInstance == null) { return; }
            if (mRecMode == false) { return; }
            Debug.LogFormat("CameraRelease");
            mCameraInstance.StopPhotoModeAsync(( result ) =>
            {
                // photo capture のリソースをシャットダウンします
                mCameraInstance.Dispose();
                mCameraInstance = null;
                mRecMode = false;
                Debug.LogFormat("CameraRelease end");
                action();
            });
        }

        ~CaptureCam () {
            CameraRelease(() => { });
        }


        /// <summary>
        /// 画面合成における仮想オブジェクトの非透明度
        /// この値が大きいほど色が濃くなる。
        /// </summary>
        /// <param name="opacity"></param>
        public void SetHoloOpacity ( float opacity ) {
            mOpacity = opacity;
        }

        private static CameraParameters InitCameraParams ( Resolution res, float opacity ) {
            if (mCameraParam.cameraResolutionWidth == res.width) { return mCameraParam; }
            mCameraParam = new CameraParameters();
            mCameraParam.hologramOpacity = opacity; // 不透明度
            mCameraParam.cameraResolutionWidth = res.width;
            mCameraParam.cameraResolutionHeight = res.height;
            mCameraParam.pixelFormat = CapturePixelFormat.BGRA32;
            return mCameraParam;
        }
    }


}