package com.example.realproject;

import android.app.Activity;
import android.app.Person;
import android.content.Intent;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;

public class UsersAdapter extends RecyclerView.Adapter<UsersAdapter.CustomViewHolder> {
    private String TAG = "adater";
    private ArrayList<PersonalData> mList = null;
    private Activity context = null;

    public UsersAdapter(Activity context, ArrayList<PersonalData> list)
    {
        this.context = context;
        this.mList = list;
    }

    class CustomViewHolder extends RecyclerView.ViewHolder{
        protected TextView notify;
        protected TextView date;
        protected TextView url;



        public CustomViewHolder(View view)
        {
            super(view);

            this.notify = (TextView) view.findViewById(R.id.textView_list_notify);
            this.date = (TextView) view.findViewById(R.id.textView_list_date);
            this.url = (TextView) view.findViewById(R.id.textView_list_link);
            itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    WebViewActivity WVA = new WebViewActivity();
                    int pos = getAdapterPosition() ;
                    String linkurl = "";
                    linkurl= mList.get(pos).getLink();

                    if (pos != RecyclerView.NO_POSITION) {
                        pos = pos+1;
                        //Toast.makeText(MainActivity.mContext, pos +" 선택됨", Toast.LENGTH_LONG).show();

                        Toast.makeText(MainActivity.mContext, linkurl +" 선택됨", Toast.LENGTH_LONG).show();
                        Intent intent = new Intent( MainActivity.mContext, WebViewActivity.class);
                        intent.putExtra("linkurl", linkurl);
                        context.startActivity(intent);
                    }
                }
            });

        }
    }

    public CustomViewHolder onCreateViewHolder(ViewGroup viewGroup, int viewType)
    {
        View view = LayoutInflater.from(viewGroup.getContext()).inflate(R.layout.item_list, null);
        CustomViewHolder viewHolder = new CustomViewHolder(view);

        return viewHolder;
    }


    public void onBindViewHolder(@NonNull CustomViewHolder viewholder, int position)
    {
        viewholder.notify.setText(mList.get(position).getNotify_list());
        viewholder.date.setText(mList.get(position).getTimestamp());
        viewholder.url.setText(mList.get(position).getLink());
    }




    @Override
    public int getItemCount() {
        Log.d(TAG, "getItemCount: " + mList.size());
        return mList.size();
    }

}