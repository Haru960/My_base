
<?php

    error_reporting(E_ALL); 
    ini_set('display_errors',1); 

    include('dbcon.php');
    $android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");

if( ($_SERVER['REQUEST_METHOD'] == 'POST') && isset($_POST['submit']) || $android ) 
    {
        $token=$_POST['Token'];
        $keyword=$_POST['keyword'];
        $OOF=$_POST['OOF'];
    if($keyword === null)
    {
        $keyword = NULL;
    }

        if(empty($token)){
            $errMSG = "이름을 입력하세요.";
        }
        else if(empty($keyword)){
            $errMSG = "나라를 입력하세요.";
        }

        if(!isset($errMSG))
        {
            try{
                $stmt = $con->prepare('select * from users where token = :token');
                $stmt->bindParam(':token', $token);
                $stmt->execute();
                
                if($stmt->rowCount() == 0){
                    $stmt = $con->prepare('INSERT INTO users(token, keyword, OOF) VALUES(:token, :keyword, :OOF)');
                    $stmt->bindParam(':token', $token);
                    $stmt->bindParam(':keyword', $keyword);
                    $stmt->bindParam(':OOF', $OOF);

                    if($stmt->execute())
                    {
                        $successMSG = "새로운 사용자를 추가했습니다.";
                    }
                    else
                    {
                        $errMSG = "사용자 추가 에러";
                    }
               }else{
                    if($stmt = $con -> prepare('UPDATE users SET keyword=:keyword, OOF=:OOF WHERE token=:token')) {
                    $stmt->bindParam(':keyword', $keyword);
                    $stmt->bindParam(':OOF', $OOF);
                    $stmt->bindParam(':token', $token);
                    $stmt -> execute();
                    }
                    if($stmt->execute())
                    {
                        $successMSG = "사용자를 업데이트 했습니다.";
                    }
                    else
                    {
                        $errMSG = "사용자 업데이트 에러";
                    }
              }
                

            } catch(PDOException $e) {
                die("Database error: " . $e->getMessage()); 
            }
        }

    }
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