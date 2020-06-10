package com.example.realproject;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.os.Bundle;
import android.util.Log;
import android.util.SparseBooleanArray;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;


import java.util.ArrayList;

public class KeywordActivity extends AppCompatActivity {
    public static final String KEYSHARED_PREFS = "keySharedPrefs";
    public static final String REMEMBERPOS = "rememberpos";
    int rePos;

    private ListView mkeywordListView;
    ArrayList<String> list = new ArrayList<String>();
    ArrayAdapter<String> adapter;


    SQLiteDatabase db;
    DatabaseHelper myDB;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_keyword);


        mkeywordListView = (ListView) findViewById(R.id.lv_keyword);

        Button btn_del = (Button) findViewById(R.id.btn_del);
        Button btn_commit = findViewById(R.id.btn_commit);

        mkeywordListView.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
        mkeywordListView.setAdapter(adapter);

        myDB = new DatabaseHelper(this, "mylist.db", null, 1);

        Cursor data = myDB.getListContents();

        if(data.getCount() == 0)
        {
            Toast.makeText(KeywordActivity.this, "비어 있음 :(", Toast.LENGTH_LONG).show();
        }
        else{
            while (data.moveToNext()){
                list.add(data.getString(1));
                adapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_single_choice, list);
                mkeywordListView.setAdapter(adapter);
            }
        }

        mkeywordListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                String item = list.get(position);
                saveData();
                loadData();
                updateData();

                Toast.makeText(KeywordActivity.this, "선택항목 : " + item, Toast.LENGTH_SHORT).show();
            }
        });
        btn_commit.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                int cntChoice = mkeywordListView.getCount();
                String checked = "";
                String unchecked = "";

                SparseBooleanArray sparseBooleanArray = mkeywordListView.getCheckedItemPositions();

                Intent intent = new Intent(KeywordActivity.this, MainActivity.class);

                for(int i = 0; i < cntChoice; i++)
                {

                    if(sparseBooleanArray.get(i) == true)
                    {
                        checked += mkeywordListView.getItemAtPosition(i).toString();
                    }
                    else  if(sparseBooleanArray.get(i) == false)
                    {
                        unchecked+= mkeywordListView.getItemAtPosition(i).toString() ;
                    }

                }

                intent.putExtra("checkedKeyword", checked);
                startActivity(intent);
            }
        });

        btn_del.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                int pos = mkeywordListView.getCheckedItemPosition();
                int cntChoice = mkeywordListView.getCount();

                String checked = "";
                String unchecked = "";

                SparseBooleanArray sparseBooleanArray = mkeywordListView.getCheckedItemPositions();
                if(pos == -1)
                {
                    Toast.makeText(KeywordActivity.this, "선택하고 삭제 버튼 누르세요", Toast.LENGTH_SHORT).show();
                }
                else
                {
                    for(int i = 0; i < cntChoice; i++)
                    {

                        if(sparseBooleanArray.get(i) == true)
                        {
                            checked += mkeywordListView.getItemAtPosition(i).toString();
                        }
                        else  if(sparseBooleanArray.get(i) == false)
                        {
                            unchecked+= mkeywordListView.getItemAtPosition(i).toString() ;
                        }

                    }



                    delete(checked);
                    list.remove(pos);
                    adapter.notifyDataSetChanged();
                    mkeywordListView.clearChoices();
                }
            }
        });
        loadData();
        updateData();
    }
    public void delete (String k)
    {
        db = myDB.getWritableDatabase();
        db.delete("MYLIST", "ITEM1=?",new String[]{k});
        Log.i("db", k + "정상적으로 삭제 됨");
    }
    public void saveData(){
        SharedPreferences sharedPreferences = getSharedPreferences(KEYSHARED_PREFS, MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putInt(REMEMBERPOS, mkeywordListView.getCheckedItemPosition());
        editor.apply();

    }
    public void loadData(){
        SharedPreferences sharedPreferences = getSharedPreferences(KEYSHARED_PREFS, MODE_PRIVATE);
        rePos = sharedPreferences.getInt(REMEMBERPOS, -1);
    }

    public void updateData(){
        mkeywordListView.setItemChecked(rePos, true);
    }



}
