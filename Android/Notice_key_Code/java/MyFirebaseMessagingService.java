package com.example.realproject;
import android.annotation.SuppressLint;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.PowerManager;
import android.util.Log;

import androidx.core.app.NotificationCompat;

import com.google.firebase.messaging.RemoteMessage;

import java.util.Map;


public class MyFirebaseMessagingService extends com.google.firebase.messaging.FirebaseMessagingService {
    private static final String TAG = "MyFirebase";

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {  //data payload로 보내면 실행

        String message = "";
        String title = "";

        // Notifition 항목이 있을때.
        if (remoteMessage.getNotification() != null) {
            message = remoteMessage.getNotification().getBody();
            title = remoteMessage.getNotification().getTitle();
        }

        // Data 항목이 있을때.
        Map<String, String> data = remoteMessage.getData();
        String messageData = data.get("body");
        String titleData = data.get("title");
        String nameData = data.get("name");

        //저는 포그라운드 백그라운드 동일하게 컨트롤하기 위해 Data항목에 푸쉬 Title, Body 모두 넣어서 구현하였습니다.
        sendNotification(titleData, messageData, nameData);


        // Check if message contains a notification payload.
        if (remoteMessage.getNotification() != null) {
            Log.d(TAG, "Message Notification Body: " + remoteMessage.getNotification().getBody());

//            String title = remoteMessage.getNotification().getTitle();
//            String body = remoteMessage.getNotification().getBody();
//            String click_action = remoteMessage.getNotification().getClickAction();
//            sendNotification(title, body, click_action);
        }
    }

    private void sendNotification(String title, String message, String name) {
        if (title == null){
            //제목이 없는 payload이면 php에서 보낼때 이미 한번 점검했음.
            title = "공지사항"; //기본제목을 적어 주자.
        }
        Intent intent;
        PendingIntent pendingIntent;

        intent = new Intent(this, MainActivity.class);
        intent.putExtra("name", name);  //push 정보중 name 값을 mainActivity로 넘김

        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        pendingIntent = PendingIntent.getActivity(this, 0 /* Request code */, intent, PendingIntent.FLAG_ONE_SHOT);


        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);

        NotificationCompat.Builder notificationBuilder;
        NotificationManager notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        //SDK26부터 푸쉬에 채널항목에 대한 세팅이 필요하다.
        if (Build.VERSION.SDK_INT >= 26) {

            String channelId = "test push";
            String channelName = "test Push Message";
            String channelDescription = "New test Information";
            NotificationChannel channel = new NotificationChannel(channelId, channelName, NotificationManager.IMPORTANCE_DEFAULT);
            channel.setDescription(channelDescription);
            //각종 채널에 대한 설정
            channel.enableLights(true);
            channel.setLightColor(Color.RED);
            channel.enableVibration(true);
            channel.setVibrationPattern(new long[]{100, 200, 300});
            notificationManager.createNotificationChannel(channel);
            //channel이 등록된 builder
            notificationBuilder = new NotificationCompat.Builder(this, channelId);
        } else {
            notificationBuilder = new NotificationCompat.Builder(this);
        }

        notificationBuilder.setSmallIcon(R.drawable.ic_launcher_foreground)
                .setContentTitle(title)
                .setStyle(new NotificationCompat.BigTextStyle().bigText(message))
                .setAutoCancel(true)
                .setSound(defaultSoundUri)
                .setContentIntent(pendingIntent)
                .setContentText(message);

        notificationManager.notify(9999 /* ID of notification */, notificationBuilder.build());
    }

}
