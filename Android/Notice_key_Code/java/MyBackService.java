package com.example.realproject;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.ProgressDialog;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.util.Log;
import android.widget.RemoteViews;
import android.widget.Toast;

import androidx.core.app.NotificationCompat;
import androidx.recyclerview.widget.RecyclerView;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;

public class MyBackService extends Service {
    private String IP_ADDRESS = "45.119.146.186";
    private ArrayList<PersonalData> mArrayList;
    private UsersAdapter mAdapter;

    public static final String NOTIFICATION_CHANNEL_ID = "10001";
    public static final String OLD = "ondOne";
    private int oldone = 0;
    private int newone = 0;

    public static final String BACK_SERVICE = "MYBACKSERVICE";
    private String mJsonString;

    private String TAG = "phpexample";

    final Handler handler = new Handler();
    public MyBackService() {
    }

    @Override
    public IBinder onBind(Intent intent) {
        // TODO: Return the communication channel to the service.
        throw new UnsupportedOperationException("Not yet implemented");
    }

    @Override
    public void onCreate() {
        super.onCreate();

        mArrayList = new ArrayList<>();
        mAdapter = new UsersAdapter(((MainActivity)MainActivity.mContext), mArrayList);
        SharedPreferences sharedPreferences = getSharedPreferences(BACK_SERVICE, MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();

        mArrayList.clear();
        mAdapter.notifyDataSetChanged();
        GetData task = new GetData();
        task.execute("http://"+IP_ADDRESS+"/getjson.php", "");
        mAdapter.getItemCount();
        Toast.makeText(MainActivity.mContext, "설정된 키워드 : "+mAdapter.getItemCount(), Toast.LENGTH_SHORT).show();


    }
    private class GetData extends AsyncTask<String, Void, String>
    {
        String errorString = null;

        protected void onPreExecute()
        {

            super.onPreExecute();


        }

        @Override
        protected void onPostExecute(String result) {
            super.onPostExecute(result);
            Log.d(TAG, "response -" + result );
            mJsonString = result;
        }

        @Override
        protected String doInBackground(String... params) {

            String serverURL = params[0];
            String postParameters = "keyword=" + params[1];


            try {

                URL url = new URL(serverURL);
                HttpURLConnection httpURLConnection = (HttpURLConnection) url.openConnection();


                httpURLConnection.setReadTimeout(5000);
                httpURLConnection.setConnectTimeout(5000);
                httpURLConnection.setRequestMethod("POST");
                httpURLConnection.setDoInput(true);
                httpURLConnection.connect();


                OutputStream outputStream = httpURLConnection.getOutputStream();
                outputStream.write(postParameters.getBytes("UTF-8"));
                outputStream.flush();
                outputStream.close();


                int responseStatusCode = httpURLConnection.getResponseCode();
                Log.d(TAG, "response code - " + responseStatusCode);

                InputStream inputStream;
                if(responseStatusCode == HttpURLConnection.HTTP_OK) {
                    inputStream = httpURLConnection.getInputStream();
                }
                else{
                    inputStream = httpURLConnection.getErrorStream();
                }


                InputStreamReader inputStreamReader = new InputStreamReader(inputStream, "UTF-8");
                BufferedReader bufferedReader = new BufferedReader(inputStreamReader);

                StringBuilder sb = new StringBuilder();
                String line;

                while((line = bufferedReader.readLine()) != null){
                    sb.append(line);
                }

                bufferedReader.close();

                return sb.toString().trim();


            } catch (Exception e) {

                Log.d(TAG, "InsertData: Error ", e);
                errorString = e.toString();

                return null;
            }

        }
    }

    //백그라운드에서 돌아갈 함수넣는 곳
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {

        handler.post(runnableCode);
        return super.onStartCommand(intent, flags, startId);
    }

    //서비스 끝
    @Override
    public void onDestroy() {
        super.onDestroy();
    }

    Runnable runnableCode = new Runnable() {
        int K = 0;
        @Override
        public void run() {
            if(K== 0) {
                GetData task = new GetData();
                task.execute( "http://" + IP_ADDRESS + "/query.php.php", "");
                newone=mAdapter.getItemCount();
                oldone = newone;
                K++;
            }
            else if(K == 1)
            {
                GetData task = new GetData();
                task.execute( "http://" + IP_ADDRESS + "/query.php.php", "");
                oldone =newone;
                newone=mAdapter.getItemCount();
                K++;
            }
            else if(oldone < newone)
            {
                GetData task = new GetData();
                task.execute( "http://" + IP_ADDRESS + "/query.php.php", "");
                NotificationSomethings();
                oldone =newone;
                newone=mAdapter.getItemCount();
                K++;
            }

        }

    };
    public void NotificationSomethings() {

        NotificationManager notificationManager = (NotificationManager)getSystemService(Context.NOTIFICATION_SERVICE);

        Intent notificationIntent = new Intent(MainActivity.mContext, MainActivity.class);
        notificationIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK) ;
        PendingIntent pendingIntent = PendingIntent.getActivity(MainActivity.mContext, 0, notificationIntent,  PendingIntent.FLAG_UPDATE_CURRENT);

        NotificationCompat.Builder builder = new NotificationCompat.Builder(MainActivity.mContext, NOTIFICATION_CHANNEL_ID)
                .setSmallIcon(R.mipmap.ic_launcher) //BitMap 이미지 요구
                .setContentTitle("설정한 키워드와 관련된 공지 감지")
                .setContentText("<<<<<눌러서 확인>>>>>")
                // 더 많은 내용이라서 일부만 보여줘야 하는 경우 아래 주석을 제거하면 setContentText에 있는 문자열 대신 아래 문자열을 보여줌
                //.setStyle(new NotificationCompat.BigTextStyle().bigText("더 많은 내용을 보여줘야 하는 경우..."))
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                .setContentIntent(pendingIntent) // 사용자가 노티피케이션을 탭시 ResultActivity로 이동하도록 설정
                .setAutoCancel(true);

        //OREO API 26 이상에서는 채널 필요
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {

            builder.setSmallIcon(R.drawable.ic_launcher_foreground); //mipmap 사용시 Oreo 이상에서 시스템 UI 에러남
            CharSequence channelName  = "노티페케이션 채널";
            String description = "오레오 이상을 위한 것임";
            int importance = NotificationManager.IMPORTANCE_HIGH;

            NotificationChannel channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName , importance);
            channel.setDescription(description);

            // 노티피케이션 채널을 시스템에 등록
            assert notificationManager != null;
            notificationManager.createNotificationChannel(channel);

        }else builder.setSmallIcon(R.mipmap.ic_launcher); // Oreo 이하에서 mipmap 사용하지 않으면 Couldn't create icon: StatusBarIcon 에러남

        assert notificationManager != null;
        notificationManager.notify(1234, builder.build()); // 고유숫자로 노티피케이션 동작시킴

    }


}
