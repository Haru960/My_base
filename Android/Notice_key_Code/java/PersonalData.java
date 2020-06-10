package com.example.realproject;

public class PersonalData {
    private String notify_list;
    private String timestamp;
    private String link;

    public String getNotify_list()
    {
        return notify_list;
    }
    public String getTimestamp()
    {
        return timestamp;
    }
    public String getLink(){
        return  link;
    }

    public void setNotify_list(String notify)
    {
        this.notify_list = notify;
    }
    public void setTimestamp(String date)
    {
        this.timestamp = date;
    }
    public void setLink(String link) { this.link = link; }
}
