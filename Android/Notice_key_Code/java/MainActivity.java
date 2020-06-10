package com.example.realproject;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.os.AsyncTask;
import android.app.ProgressDialog;
import android.os.Bundle;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnSuccessListener;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.InstanceIdResult;
import com.google.firebase.messaging.FirebaseMessaging;

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

public class MainActivity extends AppCompatActivity {
    private String IP_ADDRESS = "45.119.146.186";
    private String TAG = "phpexample";
    private String KeywordR;

    public static final String SHARED_PREFS = "sharedPrefs";
    public static final String KEYWORDSWITCH = "keywordSwitch";
    public static final String NOTIFYSWITCH = "notifySwitch";
    public static final String KEYWORDTEXT = "keywordText";

    private boolean switchOnOffkey;
    private boolean switchOnoffnotify;
    private String keyword12;

    private SharedPreferences preferences;
    private SharedPreferences.Editor editor;


    public static Context mContext;

    private TextView mTextviewRealProject;
    private RecyclerView mRecyclerView;
    private ArrayList<PersonalData> mArrayList;
    private UsersAdapter mAdapter;
    private String mJsonString;
    private Switch keywordSwitch;
    private Switch notifySwitch;


    //데이터베이스
    SQLiteDatabase db;
    DatabaseHelper myDB;
    EditText mEditText;
    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mContext = this;
        mTextviewRealProject = findViewById(R.id.tv_RealProject);
        mRecyclerView = findViewById(R.id.ReV_main);
        mRecyclerView.setLayoutManager(new LinearLayoutManager(this));
        mArrayList = new ArrayList<>();
        mAdapter = new UsersAdapter(MainActivity.this, mArrayList);
        mRecyclerView.setAdapter(mAdapter);

        //데이터 베이스 관련
        myDB = new DatabaseHelper(MainActivity.this, "mylist.db", null, 1);
        //파이어베이스!!!!!
        FirebaseMessaging.getInstance().subscribeToTopic("news");

        db = myDB.getWritableDatabase();
        //키워드 관련
        mEditText = (EditText) findViewById(R.id.et_Keywordaddmain);
        Button btn_add = findViewById(R.id.btn_add);
        Button btn_keywordlist = findViewById(R.id.btn_keywordlist);


        //preferences 환경과 관련 객체
        preferences = PreferenceManager.getDefaultSharedPreferences(this);
        //수정을 위한 editor 객체
        editor = preferences.edit();

        Button btn_keyword = findViewById(R.id.btn_keywordlist);
        //여기부터 선언끝
        btn_keyword.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(getApplicationContext(), KeywordActivity.class);
                startActivity(intent);
            }
        });
        //키워드 추가 버튼 실행
        btn_add.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                String newEntry = mEditText.getText().toString();
                int keyitems;
                if(mEditText.length() != 0)
                {
                    keyitems = count();
                    if(keyitems < 5)
                    {
                        insert(newEntry);
                        mEditText.setText("");
                        keyitems = count();
                        Toast.makeText(MainActivity.this, "현재 키워드 수 : "+ keyitems, Toast.LENGTH_LONG).show();
                    }
                    else
                    {
                        Toast.makeText(MainActivity.this, "키워드 수 5개가 꽉찼습니다.", Toast.LENGTH_LONG).show();
                    }

                }
                else{
                    Toast.makeText(MainActivity.this, "값을 넣어요", Toast.LENGTH_LONG).show();
                }
            }
        });
        //키워드 목록 확인
        btn_keywordlist.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, KeywordActivity.class);
                startActivity(intent);
            }
        });

        //리프레쉬
        Button btn_represh = findViewById(R.id.btn_refresh);
        btn_represh.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                SharedPreferences sharedPreferences = getSharedPreferences(SHARED_PREFS, MODE_PRIVATE);

                if(keywordSwitch.isChecked() == false)
                {
                    mArrayList.clear();
                    mAdapter.notifyDataSetChanged();
                    GetData task = new GetData();
                    task.execute("http://"+IP_ADDRESS+"/getjson.php", "");
                    handler.postDelayed(new Runnable() {
                        @Override
                        public void run() {
                            int i = mAdapter.getItemCount();
                            Toast.makeText(MainActivity.this, "총 공지 갯수 : "+i, Toast.LENGTH_LONG).show();
                        }
                    }, 4000);

                }
                else
                {
                    if(KeywordR == null)
                    {
                        Toast.makeText(MainActivity.this, "키워드 메뉴에서 뭔가 선택하세요", Toast.LENGTH_SHORT).show();
                    }
                    else
                    {
                        mArrayList.clear();
                        mAdapter.notifyDataSetChanged();
                        GetData task = new GetData();
                        KeywordR = sharedPreferences.getString(KEYWORDTEXT, "");
                        task.execute( "http://" + IP_ADDRESS + "/query.php", KeywordR);
                        int i = mAdapter.getItemCount();

                    }
                }
            }
        });

        //리사이클 클릭시 일어나는 이벤트


        keywordSwitch = findViewById(R.id.KeywordSwitch);
        notifySwitch =  findViewById(R.id.notifySwitch);

        loadData();
        updateData();

        keywordSwitch.setOnCheckedChangeListener(new keywordSwitchListener());
        notifySwitch.setOnCheckedChangeListener(new notifySwitchListener());


        //handler.post(runnableCode);
    }



    private class GetData extends AsyncTask<String, Void, String>
    {
        ProgressDialog progressDialog;
        String errorString = null;

        protected void onPreExecute()
        {

            super.onPreExecute();

            progressDialog = ProgressDialog.show(MainActivity.this, "Please Wait", null, true, true);

        }

        @Override
        protected void onPostExecute(String result) {
            super.onPostExecute(result);

            progressDialog.dismiss();
            mTextviewRealProject.setText(result);
            Log.d(TAG, "response -" + result );

            if(result == null)
            {
                mTextviewRealProject.setText(errorString);
            }
            else
            {
                mJsonString = result;
                showResult();
            }
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
    class InsertData extends AsyncTask<String, Void, String>{
        ProgressDialog progressDialog;

        @Override
        protected void onPreExecute() {
            super.onPreExecute();

            progressDialog = ProgressDialog.show(MainActivity.this,
                    "Please Wait", null, true, true);
        }


        @Override
        protected void onPostExecute(String result) {
            super.onPostExecute(result);

            progressDialog.dismiss();
            mTextviewRealProject.setText(result);
            Log.d(TAG, "POST response  - " + result);
        }


        @Override
        protected String doInBackground(String... params) {

            String Token = (String)params[1];
            String keyword = (String)params[2];
            String OOF = (String)params[3];
            String serverURL = (String)params[0];
            String postParameters = "Token=" + Token + "&keyword=" + keyword + "&OOF=" + OOF;


            try {

                URL url = new URL(serverURL);
                HttpURLConnection httpURLConnection = (HttpURLConnection) url.openConnection();


                httpURLConnection.setReadTimeout(5000);
                httpURLConnection.setConnectTimeout(5000);
                httpURLConnection.setRequestMethod("POST");
                httpURLConnection.connect();


                OutputStream outputStream = httpURLConnection.getOutputStream();
                outputStream.write(postParameters.getBytes("UTF-8"));
                outputStream.flush();
                outputStream.close();


                int responseStatusCode = httpURLConnection.getResponseCode();
                Log.d(TAG, "POST response code - " + responseStatusCode);

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
                String line = null;

                while((line = bufferedReader.readLine()) != null){
                    sb.append(line);
                }


                bufferedReader.close();


                return sb.toString();


            } catch (Exception e) {

                Log.d(TAG, "InsertData: Error ", e);

                return new String("Error: " + e.getMessage());
            }

        }
    }


    //디버깅용
    private void showResult()
    {
        String TAG_JSON = "webnautes";
        String TAG_NOTIFY = "notify";
        String TAG_DATE = "date";
        String TAG_link = "link";


        try {
            JSONObject jsonObject = new JSONObject(mJsonString);
            JSONArray jsonArray = jsonObject.getJSONArray(TAG_JSON);

            for(int i =0;i<jsonArray.length(); i++)
            {

                JSONObject item = jsonArray.getJSONObject(i);

                String notify = item.getString(TAG_NOTIFY);
                String date = item.getString(TAG_DATE);
                String link = item.getString(TAG_link);

                PersonalData personalData = new PersonalData();

                personalData.setNotify_list(notify);
                personalData.setTimestamp(date);
                personalData.setLink(link);

                mArrayList.add(personalData);
                mAdapter.notifyDataSetChanged();
            }

        } catch (JSONException e) {
            Log.d(TAG,"showResult : ", e);
        }
    }
    public void insert(String k)
    {
        ContentValues values = new ContentValues();
        values.put("ITEM1", k);
        db.insert("MYLIST", null,values);
    }
    public int count(){
        int cnt = 0;
        Cursor cursor = db.rawQuery("select * from MYLIST", null);
        cnt = cursor.getCount();
        return cnt;
    }

    private class keywordSwitchListener implements CompoundButton.OnCheckedChangeListener {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if(isChecked)
                {
                    saveData();
                    mArrayList.clear();
                    mAdapter.notifyDataSetChanged();
                    FirebaseMessaging.getInstance().subscribeToTopic("news");

                    final SharedPreferences sharedPreferences = getSharedPreferences(SHARED_PREFS, MODE_PRIVATE);

                    Intent intent = getIntent();
                    if(intent.getStringExtra("checkedKeyword") != null)
                    {
                        KeywordR =  intent.getStringExtra("checkedKeyword");
                        saveData();
                    }

                    Toast.makeText(MainActivity.this, "설정된 키워드 : "+KeywordR, Toast.LENGTH_SHORT).show();


                    if(KeywordR == null)
                    {
                        Toast.makeText(MainActivity.this, "키워드 메뉴에서 뭔가 선택하세요", Toast.LENGTH_SHORT).show();
                    }
                    else
                    {
                        //Toast.makeText(MainActivity.this, "선택됨"+KeywordR, Toast.LENGTH_SHORT).show();
                        GetData task = new GetData();
                        task.execute( "http://" + IP_ADDRESS + "/query.php", KeywordR);
                    }
                    if(notifySwitch.isChecked() == true){
                        FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                            @Override
                            public void onSuccess (InstanceIdResult instanceIdResult) {
                                String newToken = instanceIdResult.getToken ();
                                Log.e ( "newToken" , newToken);
                                loadData();
                                updateData();
                                saveData();
                                InsertData task = new InsertData();
                                task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, KeywordR, "1");
                            }
                        });
                    }else{
                        FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                            @Override
                            public void onSuccess (InstanceIdResult instanceIdResult) {
                                String newToken = instanceIdResult.getToken ();
                                Log.e ( "newToken" , newToken);
                                loadData();
                                updateData();
                                saveData();
                                InsertData task = new InsertData();
                                task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, KeywordR, "0");
                            }
                        });
                    }
                }
                else
                {
                    saveData();
                    mArrayList.clear();
                    mAdapter.notifyDataSetChanged();

                    GetData task = new GetData();
                    task.execute("http://"+IP_ADDRESS+"/getjson.php", "");
                    if(notifySwitch.isChecked() == true){
                        FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                            @Override
                            public void onSuccess (InstanceIdResult instanceIdResult) {
                                String newToken = instanceIdResult.getToken ();
                                Log.e ( "newToken" , newToken);
                                loadData();
                                updateData();
                                saveData();
                                InsertData task = new InsertData();
                                task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, "nullstr", "1");
                            }
                        });
                    }else{
                        FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                            @Override
                            public void onSuccess (InstanceIdResult instanceIdResult) {
                                String newToken = instanceIdResult.getToken ();
                                Log.e ( "newToken" , newToken);
                                loadData();
                                updateData();
                                saveData();
                                InsertData task = new InsertData();
                                task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, "nullstr", "0");
                            }
                        });
                    }
                }
            }
    }

    private class notifySwitchListener implements CompoundButton.OnCheckedChangeListener {

        @Override
        public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {

            if(isChecked) {
                //스위치 켜지면 60초에 한번 백그라운드 에서리싸이클뷰 업데이트
                saveData();
                FirebaseMessaging.getInstance().subscribeToTopic("news");

                if(keywordSwitch.isChecked() == true){
                    FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                        @Override
                        public void onSuccess (InstanceIdResult instanceIdResult) {
                            String newToken = instanceIdResult.getToken ();
                            Log.e ( "newToken" , newToken);
                            loadData();
                            updateData();
                            saveData();
                            InsertData task = new InsertData();
                            task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, KeywordR, "1");
                        }
                    });
                } else{
                    FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                        @Override
                        public void onSuccess (InstanceIdResult instanceIdResult) {
                            String newToken = instanceIdResult.getToken ();
                            Log.e ( "newToken" , newToken);
                            loadData();
                            updateData();
                            saveData();
                            InsertData task = new InsertData();
                            task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, "nullstr", "1");
                        }
                    });
                }
            }
            else{
                saveData();
                FirebaseMessaging.getInstance().subscribeToTopic("news");
                if(keywordSwitch.isChecked() == true){
                    FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                        @Override
                        public void onSuccess (InstanceIdResult instanceIdResult) {
                            String newToken = instanceIdResult.getToken ();
                            Log.e ( "newToken" , newToken);
                            loadData();
                            updateData();
                            saveData();
                            Toast.makeText(MainActivity.this, "설정된 키워드 : "+KeywordR, Toast.LENGTH_SHORT).show();
                            InsertData task = new InsertData();
                            task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, KeywordR, "0");
                        }
                    });
                } else{
                    FirebaseInstanceId.getInstance (). getInstanceId (). addOnSuccessListener (MainActivity.this, new OnSuccessListener<InstanceIdResult>() {
                        @Override
                        public void onSuccess (InstanceIdResult instanceIdResult) {
                            String newToken = instanceIdResult.getToken ();
                            Log.e ( "newToken" , newToken);
                            loadData();
                            updateData();
                            saveData();
                            InsertData task = new InsertData();
                            task.execute("http://" + IP_ADDRESS + "/insert.php", newToken, "nullstr", "0");
                        }
                    });
                }
            }
        }
    }

    public void saveData(){
        SharedPreferences sharedPreferences = getSharedPreferences(SHARED_PREFS, MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();

        editor.putBoolean(KEYWORDSWITCH, keywordSwitch.isChecked());
        editor.putBoolean(NOTIFYSWITCH, notifySwitch.isChecked());
        editor.putString(KEYWORDTEXT,KeywordR);
        editor.apply();

        //Toast.makeText(MainActivity.this, keyword12+"", Toast.LENGTH_SHORT).show();

    }
    public void loadData(){
        SharedPreferences sharedPreferences = getSharedPreferences(SHARED_PREFS, MODE_PRIVATE);
        //getBoolean("가져올 값의 키", 데이터가 없을시 가져올 값)
        switchOnOffkey = sharedPreferences.getBoolean(KEYWORDSWITCH, false);
        switchOnoffnotify = sharedPreferences.getBoolean(NOTIFYSWITCH, false);
        keyword12 = sharedPreferences.getString(KEYWORDTEXT, "");
        //Toast.makeText(MainActivity.this, "LoadData"+keyword12+","+switchOnOffkey, Toast.LENGTH_SHORT).show();
    }

    public void updateData(){
        keywordSwitch.setChecked(switchOnOffkey);
        notifySwitch.setChecked(switchOnoffnotify);
        KeywordR = keyword12;
    }



    final Handler handler = new Handler();
    Runnable runnableCode = new Runnable() {
        @Override
        public void run() {
            mArrayList.clear();
            mAdapter.notifyDataSetChanged();

            GetData task = new GetData();
            task.execute( "http://" + IP_ADDRESS + "/getjson.php", "");

            handler.postDelayed(this, 5000);

            mAdapter.registerAdapterDataObserver(new RecyclerView.AdapterDataObserver() {
                @Override
                public void onChanged() {
                    super.onChanged();

                }

            });
        }
    };


}

