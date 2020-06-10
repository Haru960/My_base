<?php

    error_reporting(E_ALL); 
    ini_set('display_errors',1); 

    include('dbcon.php');
    $android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");

    if( ($_SERVER['REQUEST_METHOD'] == 'POST') && isset($_POST['submit']) || $android ) {
    $token = $_POST['Token'];
    $keyword = $_POST['keyword'];
    $OOF = $_POST['OOF'];
 
   $sql = "
        INSERT INTO users(token, keyword, OOF)
        VALUES ('$token','$keyword',$OOF);
        "
        //write_log("./log_sql.txt", $sql);
   $result = mysqli_query($con, $sql);
    if($result)
    {
        echo ('Successfully Savedd');
    }   
    else{
        echo('Not saved Successfully');
    }
}
      mysqli_close($con);
?>

<html>
   <body>
        <?php 
        if (isset($errMSG)) echo $errMSG;
        if (isset($successMSG)) echo $successMSG;
        ?>
        
        <form action="<?php $_PHP_SELF ?>" method="POST">
            Token: <input type = "text" name = "Token" />
            keyword: <input type = "text" name = "keyword" />
            OOF: <input type = "text" name = "OOF" />
            <input type = "submit" name = "submit" />
        </form>
   
   </body>
</html>