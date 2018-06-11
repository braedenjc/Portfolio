 
void Model::saveProject(){ //Braeden
    //Referencing: https://stackoverflow.com/questions/4916193/creating-writing-into-a-new-file-in-qt#4916274
    QImage *currentImage;
    std::string dummyOutput;
    dummyOutput += std::to_string(canvasSize) + " " + std::to_string(canvasSize) + "\n";

    for(uint frame = 0; frame < frames.size(); frame++){
        currentImage = frames[frame];
        dummyOutput += std::to_string(frame) + "\n";

        for(uint pixel_y = 0; pixel_y < canvasSize; pixel_y++){
            for(uint pixel_x = 0; pixel_x < canvasSize; pixel_x++){
                QColor currentPixelColor = currentImage->pixelColor(pixel_x,pixel_y);

                dummyOutput += std::to_string(currentPixelColor.red())  + " ";
                dummyOutput += std::to_string(currentPixelColor.green()) + " ";
                dummyOutput += std::to_string(currentPixelColor.blue()) + " ";
                dummyOutput += std::to_string(currentPixelColor.alpha());

                if(pixel_x < canvasSize - 1){
                    dummyOutput += " ";
                }
            }
        dummyOutput += "\n";
        }
    }

    QFile saveLocation(projectFile);
    QTextStream output(&saveLocation);
    if(saveLocation.open(QIODevice::ReadWrite))
    {
        QString tempOut = QString::fromStdString(dummyOutput);
        output << tempOut;
        std::cout << "Printing File" << std::endl;
    }
    saveLocation.close();
}

void Model::exportGif(QString filePath){
    //Give the file path the gif suffix
    filePath += ".gif";
    //Convert the qstring to a const char
    QByteArray charArray = filePath.toLatin1();
    const char *filePathChar;
    filePathChar = charArray.data();
    //create gif file and write it
    GifWriter writer;
    GifBegin(&writer, filePathChar, canvasSize, canvasSize, 0);
    for (int i = 0; i <frames.size(); i++){
        GifWriteFrame(&writer, frames.at(i)->rgbSwapped().constScanLine(0), canvasSize, canvasSize, 0);

    }
    GifEnd(&writer);
}