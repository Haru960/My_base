<?php 

    error_reporting(E_ALL); 
    ini_set('display_errors',1); 

    include('dbcon.php');


    $android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");


    if( ($_SERVER['REQUEST_METHOD'] == 'POST') && isset($_POST['submit']) || $android ) 
    {

        // 안드로이드 코드의 postParameters 변수에 적어준 이름을 가지고 값을 전달 받습니다.

        $token = $_POST['Token'];
        $keyword = $_POST['keyword'];
        $OOF = $_POST['OOF'];

        if(empty($token)){
            $errMSG = "이름을 입력";
        }
        else if (empty($keyword))
        {
            $errMSG = "키워드 누락";
        }
        else if (empty($OOF))
        {
            $errMSG = "OOF 누락";
        }


        if(!isset($errMSG)) // 이름과 나라 모두 입  력이 되었다면 
        {
            try{
                // SQL문을 실행하여 데이터를 MySQL 서버의 person 테이블에 저장합니다. 
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